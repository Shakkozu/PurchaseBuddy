using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;

public interface IShoppingListWriteService
{
	void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid listItemId, int newQuantity);
	void MarkListItemAsNotPurchased(Guid userId, Guid shoppingListId, Guid listItemId);
	void MarkListItemAsPurchased(Guid userId, Guid shoppingListId, Guid listItemId);
	void MarkListItemtAsUnavailable(Guid userId, Guid shoppingListId, Guid listItemId);
	void RemoveItemFromList(Guid userId, Guid shoppingListId, Guid listItemId);
	Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null);
	void AddNewListItem(Guid userId, Guid listId, AddNewListItemRequest addNewItemRequest);
	void GrantAccessToModifyingList(Guid listId, Guid userId);
}
