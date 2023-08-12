using MediatR;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.app.eventHandlers;
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

	public void InviteOtherUserToModifyList(Guid listCreatorId, Guid listId, Guid invitedUserId)
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

			return;
		}

		invitationsList.InviteUser(invitedUserId, list.CreatorId);
		repo.Update(invitationsList);
	}

	public void RejectAnInvite(Guid listId, Guid invitedUserId)
	{
		var invitationsList = repo.Get(listId);
		if (invitationsList == null)
			throw new ArgumentException($"Intivation with id {listId} not found");

		invitationsList.Reject(invitedUserId);
		repo.Update(invitationsList);
	}

	public void MarkInvitationAsExpired(Guid listId)
	{
		var invitationsList = repo.Get(listId);
		if (invitationsList == null)
			throw new ArgumentException($"Intivation with id {listId} not found");

		invitationsList.MarkAsExpired();
		repo.Update(invitationsList);
	}
}
