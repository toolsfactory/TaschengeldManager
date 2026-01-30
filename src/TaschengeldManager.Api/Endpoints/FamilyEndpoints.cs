using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Api.Filters;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Family management endpoints.
/// </summary>
public static class FamilyEndpoints
{
    public static IEndpointRouteBuilder MapFamilyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/family")
            .WithTags("Family")

            .RequireAuthorization();

        // Family CRUD
        group.MapPost("/", CreateFamily)
            .WithSummary("Create a new family")
            .Produces<FamilyDto>(201)
            .Produces(400);

        group.MapGet("/{id:guid}", GetFamily)
            .WithSummary("Get a family by ID")
            .Produces<FamilyDto>(200)
            .Produces(404);

        group.MapGet("/", GetFamilies)
            .WithSummary("Get all families for the current user")
            .Produces<IReadOnlyList<FamilyDto>>(200);

        // Members
        group.MapGet("/{familyId:guid}/members", GetMembers)
            .WithSummary("Get all members (parents and relatives) of a family")
            .Produces<IReadOnlyList<FamilyMemberDto>>(200);

        // Children
        group.MapGet("/{familyId:guid}/children", GetChildren)
            .WithSummary("Get all children in a family")
            .Produces<IReadOnlyList<ChildDto>>(200);

        group.MapPost("/{familyId:guid}/children", AddChild)
            .WithSummary("Add a child to a family")
            .WithValidation<AddChildRequest>()
            .Produces<ChildDto>(201)
            .Produces(400)
            .Produces(403);

        group.MapDelete("/{familyId:guid}/children/{childId:guid}", RemoveChild)
            .WithSummary("Remove a child from a family")
            .Produces(204)
            .Produces(403)
            .Produces(404);

        group.MapPut("/{familyId:guid}/children/{childId:guid}/pin", ChangeChildPin)
            .WithSummary("Change a child's PIN")
            .Produces(204)
            .Produces(400)
            .Produces(403);

        // Invitations
        group.MapPost("/{familyId:guid}/invitations", InviteToFamily)
            .WithSummary("Invite someone to the family")
            .Produces<InvitationDto>(201)
            .Produces(400)
            .Produces(403);

        group.MapGet("/{familyId:guid}/invitations", GetPendingInvitations)
            .WithSummary("Get pending invitations for a family")
            .Produces<IReadOnlyList<InvitationDto>>(200)
            .Produces(403);

        group.MapPost("/invitations/accept", AcceptInvitation)
            .WithSummary("Accept an invitation")
            .Produces(204)
            .Produces(400);

        group.MapPost("/invitations/reject", RejectInvitation)
            .WithSummary("Reject an invitation")
            .Produces(204)
            .Produces(400);

        group.MapDelete("/{familyId:guid}/invitations/{invitationId:guid}", WithdrawInvitation)
            .WithSummary("Withdraw an invitation")
            .Produces(204)
            .Produces(400)
            .Produces(403);

        // Relatives
        group.MapPost("/{familyId:guid}/relatives", CreateRelative)
            .WithSummary("Create a relative account directly")
            .Produces<FamilyMemberDto>(201)
            .Produces(400)
            .Produces(403);

        group.MapDelete("/{familyId:guid}/relatives/{relativeUserId:guid}", RemoveRelative)
            .WithSummary("Remove a relative from the family")
            .Produces(204)
            .Produces(400)
            .Produces(403);

        // Parents
        group.MapDelete("/{familyId:guid}/parents/{targetUserId:guid}", RemoveParent)
            .WithSummary("Remove a parent from the family")
            .Produces(204)
            .Produces(400)
            .Produces(403);

        return app;
    }

    private static async Task<IResult> CreateFamily(
        CreateFamilyRequest request,
        ClaimsPrincipal user,
        IFamilyService familyService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await familyService.CreateFamilyAsync(userId, request, cancellationToken);
            return Results.Created($"/api/family/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetFamily(
        Guid id,
        ClaimsPrincipal user,
        IFamilyService familyService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var family = await familyService.GetFamilyAsync(userId, new FamilyId(id), cancellationToken);

        if (family == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(family);
    }

    private static async Task<IResult> GetFamilies(
        ClaimsPrincipal user,
        IFamilyService familyService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var families = await familyService.GetFamiliesForUserAsync(userId, cancellationToken);
        return Results.Ok(families);
    }

    private static async Task<IResult> GetMembers(
        Guid familyId,
        ClaimsPrincipal user,
        IFamilyMemberService memberService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var members = await memberService.GetMembersAsync(userId, new FamilyId(familyId), cancellationToken);
        return Results.Ok(members);
    }

    private static async Task<IResult> GetChildren(
        Guid familyId,
        ClaimsPrincipal user,
        IChildManagementService childService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var children = await childService.GetChildrenAsync(userId, new FamilyId(familyId), cancellationToken);
        return Results.Ok(children);
    }

    private static async Task<IResult> AddChild(
        Guid familyId,
        AddChildRequest request,
        ClaimsPrincipal user,
        IChildManagementService childService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await childService.AddChildAsync(userId, new FamilyId(familyId), request, cancellationToken);
            return Results.Created($"/api/family/{familyId}", result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RemoveChild(
        Guid familyId,
        Guid childId,
        ClaimsPrincipal user,
        IChildManagementService childService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await childService.RemoveChildAsync(userId, new FamilyId(familyId), new UserId(childId), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
    }

    private static async Task<IResult> ChangeChildPin(
        Guid familyId,
        Guid childId,
        [FromBody] ChangeChildPinRequest request,
        ClaimsPrincipal user,
        IChildManagementService childService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await childService.ChangeChildPinAsync(userId, new FamilyId(familyId), new UserId(childId), request, cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> InviteToFamily(
        Guid familyId,
        InviteRequest request,
        ClaimsPrincipal user,
        IFamilyInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await invitationService.InviteToFamilyAsync(userId, new FamilyId(familyId), request, cancellationToken);
            return Results.Created($"/api/family/{familyId}/invitations", result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetPendingInvitations(
        Guid familyId,
        ClaimsPrincipal user,
        IFamilyInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var invitations = await invitationService.GetPendingInvitationsAsync(userId, new FamilyId(familyId), cancellationToken);
            return Results.Ok(invitations);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> AcceptInvitation(
        AcceptInvitationRequest request,
        ClaimsPrincipal user,
        IFamilyInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await invitationService.AcceptInvitationAsync(userId, request, cancellationToken);
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RejectInvitation(
        RejectInvitationRequest request,
        ClaimsPrincipal user,
        IFamilyInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await invitationService.RejectInvitationAsync(userId, request.Token, cancellationToken);
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> WithdrawInvitation(
        Guid familyId,
        Guid invitationId,
        ClaimsPrincipal user,
        IFamilyInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await invitationService.WithdrawInvitationAsync(userId, new FamilyInvitationId(invitationId), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> CreateRelative(
        Guid familyId,
        CreateRelativeRequest request,
        ClaimsPrincipal user,
        IFamilyMemberService memberService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await memberService.CreateRelativeAsync(userId, new FamilyId(familyId), request, cancellationToken);
            return Results.Created($"/api/family/{familyId}", result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RemoveRelative(
        Guid familyId,
        Guid relativeUserId,
        ClaimsPrincipal user,
        IFamilyMemberService memberService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await memberService.RemoveRelativeAsync(userId, new FamilyId(familyId), new UserId(relativeUserId), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RemoveParent(
        Guid familyId,
        Guid targetUserId,
        ClaimsPrincipal user,
        IFamilyMemberService memberService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await memberService.RemoveParentAsync(userId, new FamilyId(familyId), new UserId(targetUserId), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// Request to reject an invitation.
/// </summary>
public record RejectInvitationRequest
{
    public required string Token { get; init; }
}
