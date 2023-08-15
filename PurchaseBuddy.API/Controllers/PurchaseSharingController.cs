
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.src.auth.app;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("purchase-sharing")]
public class PurchaseSharingController : BaseController
{
	public PurchaseSharingController(SharingFacade sharingFacade,
		SharingReadFacade sharingReadFacade,
		IUserAuthorizationService authorizationService)
		: base(authorizationService)
	{
		this.sharingFacade = sharingFacade;
		this.sharingReadFacade = sharingReadFacade;
	}

	[HttpPost("shopping-lists/{listId}/invitations")]
	public async Task<IActionResult> InviteUserToShopping(Guid listId, [FromBody] InviteUserToModyfingListRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var invitationId = sharingFacade.InviteOtherUserToModifyList(user.Guid, listId, request.InvitedUserId);

		return Created("/purchase-sharing/invitations", invitationId);
	}

	[HttpPost("shopping-lists/{listId}/accept-invite")]
	public async Task<IActionResult> AcceptInvite(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		sharingFacade.AcceptAnInvite(listId, user.Guid);

		return NoContent();
	}

	[HttpPost("shopping-lists/{listId}/reject-invite")]
	public async Task<IActionResult> RejectInvite(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		sharingFacade.RejectAnInvite(listId, user.Guid);

		return NoContent();
	}

	[HttpGet("pending-invitations")]
	public async Task<IActionResult> GetAllPendingInvitations()
	{
		var user = await GetUserFromSessionAsync();
		var result = sharingReadFacade.GetAllActiveInvitationsForUser(user.Guid);

		return Ok(result);
	}

	private readonly SharingFacade sharingFacade;
	private readonly SharingReadFacade sharingReadFacade;
}

public record InviteUserToModyfingListRequest(Guid InvitedUserId);