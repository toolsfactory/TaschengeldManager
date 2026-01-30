using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service implementation for money request management.
/// </summary>
public class MoneyRequestService : IMoneyRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MoneyRequestService> _logger;

    public MoneyRequestService(IUnitOfWork unitOfWork, ILogger<MoneyRequestService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MoneyRequestDto> CreateAsync(UserId childUserId, CreateMoneyRequestRequest request, CancellationToken cancellationToken = default)
    {
        // Verify user is a child
        var child = await _unitOfWork.Users.GetByIdAsync(childUserId, cancellationToken);
        if (child == null || child.Role != UserRole.Child)
        {
            throw new UnauthorizedAccessException("Only children can create money requests");
        }

        var moneyRequest = new MoneyRequest
        {
            Id = new MoneyRequestId(Guid.NewGuid()),
            ChildUserId = childUserId,
            Amount = request.Amount,
            Reason = request.Reason,
            Status = RequestStatus.Pending
        };

        await _unitOfWork.MoneyRequests.AddAsync(moneyRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Money request {RequestId} created by child {ChildId} for {Amount}",
            moneyRequest.Id.Value, childUserId.Value, request.Amount);

        return MapToDto(moneyRequest, child.Nickname, null);
    }

    public async Task<MoneyRequestDto?> GetByIdAsync(UserId userId, MoneyRequestId requestId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MoneyRequests.GetWithDetailsAsync(requestId, cancellationToken);
        if (request == null)
        {
            return null;
        }

        // Check access - child can see their own, parents can see their family's
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        if (user.Role == UserRole.Child)
        {
            if (request.ChildUserId != userId)
            {
                return null;
            }
        }
        else if (user.Role == UserRole.Parent)
        {
            var child = request.ChildUser;
            if (child?.FamilyId == null)
            {
                return null;
            }

            var family = await _unitOfWork.Families.GetWithMembersAsync(child.FamilyId.Value, cancellationToken);
            if (family == null || !family.Parents.Any(p => p.UserId == userId))
            {
                return null;
            }
        }
        else
        {
            return null;
        }

        return MapToDto(request, request.ChildUser?.Nickname ?? "Unknown", request.RespondedByUser?.Nickname);
    }

    public async Task<IReadOnlyList<MoneyRequestDto>> GetMyRequestsAsync(UserId childUserId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(childUserId, cancellationToken);
        if (user == null || user.Role != UserRole.Child)
        {
            return [];
        }

        var requests = await _unitOfWork.MoneyRequests.GetByChildIdAsync(childUserId, cancellationToken);

        return requests.Select(r => MapToDto(r, user.Nickname, r.RespondedByUser?.Nickname)).ToList();
    }

    public async Task<IReadOnlyList<MoneyRequestDto>> GetFamilyRequestsAsync(UserId parentUserId, RequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var parent = await _unitOfWork.Users.GetByIdAsync(parentUserId, cancellationToken);
        if (parent == null || parent.Role != UserRole.Parent)
        {
            return [];
        }

        // Get family where user is a parent
        var families = await _unitOfWork.Families.GetFamiliesForParentAsync(parentUserId, cancellationToken);
        if (families.Count == 0)
        {
            return [];
        }

        // Get requests for all families (usually just one)
        var allRequests = new List<MoneyRequestDto>();
        foreach (var family in families)
        {
            var requests = await _unitOfWork.MoneyRequests.GetAllForFamilyAsync(family.Id, status, cancellationToken);
            allRequests.AddRange(requests.Select(r => MapToDto(r, r.ChildUser?.Nickname ?? "Unknown", r.RespondedByUser?.Nickname)));
        }

        return allRequests.OrderByDescending(r => r.CreatedAt).ToList();
    }

    public async Task<MoneyRequestDto> RespondAsync(UserId parentUserId, MoneyRequestId requestId, RespondToRequestRequest response, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MoneyRequests.GetWithDetailsAsync(requestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Can only respond to pending requests");
        }

        // Verify parent has access
        var child = request.ChildUser;
        if (child?.FamilyId == null)
        {
            throw new InvalidOperationException("Child not found or not in a family");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(child.FamilyId.Value, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        var parent = await _unitOfWork.Users.GetByIdAsync(parentUserId, cancellationToken);

        request.Status = response.Approve ? RequestStatus.Approved : RequestStatus.Rejected;
        request.ResponseNote = response.Note;
        request.RespondedByUserId = parentUserId;
        request.RespondedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        // If approved, create transaction
        if (response.Approve)
        {
            _logger.LogInformation("Looking for account for child {ChildId}", child.Id.Value);

            var account = await _unitOfWork.Accounts.GetByUserIdAsync(child.Id, cancellationToken);
            if (account == null)
            {
                _logger.LogError("Child account not found for user {ChildId}", child.Id.Value);
                throw new InvalidOperationException("Child account not found");
            }

            _logger.LogInformation("Found account {AccountId} with balance {Balance}", account.Id.Value, account.Balance);

            var transaction = new Transaction
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account.Id,
                Amount = request.Amount,
                Type = TransactionType.Deposit,
                Description = $"Anfrage genehmigt: {request.Reason}",
                BalanceAfter = account.Balance + request.Amount,
                CreatedByUserId = parentUserId
            };

            await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
            account.Balance += request.Amount;

            _logger.LogInformation("Created transaction {TransactionId}, new balance: {NewBalance}",
                transaction.Id.Value, account.Balance);

            request.ResultingTransactionId = transaction.Id;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Money request {RequestId} {Status} by parent {ParentId}",
            requestId.Value, request.Status, parentUserId.Value);

        return MapToDto(request, child.Nickname, parent?.Nickname);
    }

    public async Task WithdrawAsync(UserId childUserId, MoneyRequestId requestId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MoneyRequests.GetByIdAsync(requestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        if (request.ChildUserId != childUserId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Can only withdraw pending requests");
        }

        request.Status = RequestStatus.Withdrawn;
        request.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Money request {RequestId} withdrawn by child {ChildId}",
            requestId.Value, childUserId.Value);
    }

    private static MoneyRequestDto MapToDto(MoneyRequest request, string childName, string? respondedByName)
    {
        return new MoneyRequestDto
        {
            Id = request.Id.Value,
            ChildName = childName,
            ChildUserId = request.ChildUserId.Value,
            Amount = request.Amount,
            Reason = request.Reason,
            Status = request.Status,
            ResponseNote = request.ResponseNote,
            RespondedByName = respondedByName,
            RespondedAt = request.RespondedAt,
            CreatedAt = request.CreatedAt
        };
    }
}
