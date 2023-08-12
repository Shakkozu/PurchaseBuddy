using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.app;

public interface IShoppingListInvitationsRepository
{
	ShoppingInvitationsList? Get(Guid shoppingListGuid);
	IEnumerable<ShoppingInvitationsList> GetAllWhereUserIsOnInvitationsList(Guid userGuid);
	void Save(ShoppingInvitationsList shoppingList);
	void Update(ShoppingInvitationsList shoppingList);
}
