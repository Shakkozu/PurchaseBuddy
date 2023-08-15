using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.crm;

namespace PurchaseBuddy.src.purchases.app;

public class SharingReadFacade
{
	private readonly IShoppingListInvitationsRepository repo;
	private readonly IUsersProvider usersService;

	public SharingReadFacade(IShoppingListInvitationsRepository repo,
		IUsersProvider usersService)
    {
		this.repo = repo;
		this.usersService = usersService;
	}

	public List<InvitationDto> GetAllActiveInvitationsForUser(Guid userId)
	{
		var invitationsList = repo.GetAllWhereUserIsOnInvitationsList(userId);
		var users = usersService.GetAllUsersWithGuids(invitationsList.Select(x => x.CreatorId));

		var result = new List<InvitationDto>();
		foreach(var invitations in invitationsList)
		{
			var invitationForUser = invitations.GetInvitationForUser(userId);
			if (invitationForUser == null)
				continue;

			result.Add(new InvitationDto
			{
				CreatedAt = invitationForUser.CreatedAt,
				UserId = invitationForUser.UserId,
				ListCreatorId = invitations.CreatorId,
				ListCreatorName = users.First(user => user.Guid == invitations.CreatorId).Name,
				ListId = invitations.ListId
			});
		}

		return result;
	}
}
