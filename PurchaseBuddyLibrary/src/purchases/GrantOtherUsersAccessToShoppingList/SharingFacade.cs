using MediatR;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.domain;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.events;

namespace PurchaseBuddy.src.purchases.app;

public class SharingFacade
{
	private readonly IShoppingListInvitationsRepository repo;
	private readonly IShoppingListReadService shoppingListReadService;
	private readonly IMediator mediator;

	public SharingFacade(IShoppingListInvitationsRepository repo,
		IShoppingListReadService shoppingListReadService,
		IMediator mediator)
	{	
		this.repo = repo;
		this.shoppingListReadService = shoppingListReadService;
		this.mediator = mediator;
	}

	public void AcceptAnInvite(Guid listId, Guid invitedUserId)
	{
		var invitationsList = repo.Get(listId);
		if (invitationsList == null)
			throw new ArgumentException($"Invitation with id {listId} not found");
		
		invitationsList.AcceptInviteToModify(invitedUserId);
		repo.Update(invitationsList);
		mediator.Send(new InvitationToModifyingShoppingListAccepted(DateTime.Now, listId, invitedUserId));
	}

	public Guid InviteOtherUserToModifyList(Guid listCreatorId, Guid listId, Guid invitedUserId)
	{
		var invitationsList = repo.Get(listId);
		var list = shoppingListReadService.GetShoppingList(listCreatorId, listId);
		if (list == null)
			throw new ArgumentException($"List with id {listId} not found for user {listCreatorId}");

		if (invitationsList == null)
		{
			invitationsList = ShoppingInvitationsList.CreateNew(list.Guid, list.Completed.GetValueOrDefault(), list.CreatorId);
			invitationsList.InviteUser(invitedUserId, list.CreatorId);
			repo.Save(invitationsList);

			return invitationsList.Guid;
		}

		invitationsList.InviteUser(invitedUserId, list.CreatorId);
		repo.Update(invitationsList);
		return invitationsList.Guid;
	}

	public void RejectAnInvite(Guid listId, Guid invitedUserId)
	{
		var invitationsList = repo.Get(listId);
		if (invitationsList == null)
			throw new ArgumentException($"Intivation with id {listId} not found");

		invitationsList.Reject(invitedUserId);
		repo.Update(invitationsList);
	}

	public void MarkInvitationsAsExpired(Guid listId)
	{
		var invitationsList = repo.Get(listId);
		if (invitationsList == null)
			throw new ArgumentException($"Intivation with id {listId} not found");

		invitationsList.MarkAsExpired();
		repo.Update(invitationsList);
	}
}
