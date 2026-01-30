using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for family invitation operations.
/// </summary>
public class FamilyInvitationService : IFamilyInvitationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FamilyInvitationService> _logger;
    private readonly ICacheService _cache;

    public FamilyInvitationService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<FamilyInvitationService> logger,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
    }

    public async Task<InvitationDto> InviteToFamilyAsync(UserId parentUserId, FamilyId familyId, InviteRequest request, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can invite members");
        }

        if (request.Role == UserRole.Child)
        {
            throw new InvalidOperationException("Cannot invite children. Use AddChild instead.");
        }

        var parent = await _unitOfWork.Users.GetByIdAsync(parentUserId, cancellationToken);

        // Check for existing pending invitation
        var existingInvitations = await _unitOfWork.FamilyInvitations.GetPendingByEmailAsync(request.Email, cancellationToken);
        var existingInvitation = existingInvitations.FirstOrDefault(i => i.FamilyId == familyId);
        if (existingInvitation != null)
        {
            throw new InvalidOperationException("An invitation is already pending for this email");
        }

        var token = _tokenService.GenerateSecureToken();
        var invitation = new FamilyInvitation
        {
            Id = new FamilyInvitationId(Guid.NewGuid()),
            FamilyId = familyId,
            InvitedEmail = request.Email,
            NormalizedInvitedEmail = request.Email.ToUpperInvariant(),
            InvitedByUserId = parentUserId,
            InvitedRole = request.Role,
            Status = InvitationStatus.Pending,
            Token = token,
            TokenHash = _tokenService.HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RelationshipDescription = request.RelationshipDescription
        };

        await _unitOfWork.FamilyInvitations.AddAsync(invitation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send invitation email
        var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:5001";
        var invitationLink = $"{baseUrl}/invitation?token={token}";
        await _emailService.SendInvitationEmailAsync(
            request.Email,
            parent?.Nickname ?? "Unknown",
            family.Name,
            invitationLink,
            request.Role.ToString(),
            cancellationToken);

        _logger.LogInformation("Invitation sent to {Email} for family {FamilyId} as {Role}", request.Email, familyId.Value, request.Role);

        return new InvitationDto
        {
            Id = invitation.Id.Value,
            InvitedEmail = invitation.InvitedEmail,
            InvitedRole = invitation.InvitedRole,
            Status = invitation.Status,
            InvitedByName = parent?.Nickname ?? "Unknown",
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            RelationshipDescription = invitation.RelationshipDescription
        };
    }

    public async Task AcceptInvitationAsync(UserId userId, AcceptInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenService.HashToken(request.Token);
        var invitation = await _unitOfWork.FamilyInvitations.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (invitation == null || invitation.Status != InvitationStatus.Pending || invitation.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid or expired invitation");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Verify email matches
        if (!string.Equals(user.Email, invitation.InvitedEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("This invitation was sent to a different email address");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(invitation.FamilyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        // Add user to family
        if (invitation.InvitedRole == UserRole.Parent)
        {
            family.Parents.Add(new FamilyParent
            {
                FamilyId = family.Id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                IsPrimary = false
            });
        }
        else if (invitation.InvitedRole == UserRole.Relative)
        {
            family.Relatives.Add(new FamilyRelative
            {
                FamilyId = family.Id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                RelationshipDescription = invitation.RelationshipDescription
            });

            user.Role = UserRole.Relative;
        }

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        _logger.LogInformation("User {UserId} accepted invitation to family {FamilyId}", userId.Value, family.Id.Value);
    }

    public async Task RejectInvitationAsync(UserId userId, string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenService.HashToken(token);
        var invitation = await _unitOfWork.FamilyInvitations.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (invitation == null || invitation.Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Invalid invitation");
        }

        invitation.Status = InvitationStatus.Rejected;
        invitation.RespondedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invitation {InvitationId} rejected", invitation.Id.Value);
    }

    public async Task WithdrawInvitationAsync(UserId parentUserId, FamilyInvitationId invitationId, CancellationToken cancellationToken = default)
    {
        var invitation = await _unitOfWork.FamilyInvitations.GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            throw new InvalidOperationException("Invitation not found");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(invitation.FamilyId, cancellationToken);
        if (family == null || !IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can withdraw invitations");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Can only withdraw pending invitations");
        }

        invitation.Status = InvitationStatus.Withdrawn;
        invitation.RespondedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invitation {InvitationId} withdrawn by parent {ParentId}", invitationId.Value, parentUserId.Value);
    }

    public async Task<IReadOnlyList<InvitationDto>> GetPendingInvitationsAsync(UserId parentUserId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        var invitations = await _unitOfWork.FamilyInvitations.GetPendingByFamilyAsync(familyId, cancellationToken);

        return invitations.Select(i => new InvitationDto
        {
            Id = i.Id.Value,
            InvitedEmail = i.InvitedEmail,
            InvitedRole = i.InvitedRole,
            Status = i.Status,
            InvitedByName = i.InvitedByUser?.Nickname ?? "Unknown",
            CreatedAt = i.CreatedAt,
            ExpiresAt = i.ExpiresAt,
            RelationshipDescription = i.RelationshipDescription
        }).ToList();
    }

    private static bool IsParent(Family family, UserId userId)
    {
        return family.Parents.Any(p => p.UserId == userId);
    }

    private async Task InvalidateFamilyCacheAsync(Family family, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Family(family.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.FamilyByCode(family.FamilyCode), cancellationToken);

        foreach (var parent in family.Parents)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(parent.UserId), cancellationToken);
        }

        foreach (var child in family.Children)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(child.Id), cancellationToken);
        }

        foreach (var relative in family.Relatives)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(relative.UserId), cancellationToken);
        }
    }
}
