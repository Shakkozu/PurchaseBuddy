using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddy.src.purchases.app;

public class SharingReadFacade
{
	private readonly IShoppingListInvitationsRepository repo;

	public SharingReadFacade(IShoppingListInvitationsRepository repo)
    {
		this.repo = repo;
	}

	public List<InvitationDto> GetAllActiveInvitationsForUser(Guid userId)
	{
		var invitationsList = repo.GetAllWhereUserIsOnInvitationsList(userId);

		return invitationsList.Select(x => new InvitationDto
		{
			ListId = x.ListId,
			ListCreatorId = x.CreatorId,
			UserId = userId,
		}).ToList();
	}
}
