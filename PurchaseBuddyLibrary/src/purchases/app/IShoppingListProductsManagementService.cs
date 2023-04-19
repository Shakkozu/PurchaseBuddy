using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.purchases.contract;

namespace PurchaseBuddy.src.purchases.app;
public interface IShoppingListService
{
	void AddProductToList(Guid userId, Guid shoppingListId, UserProduct userProduct);
	void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid productId, int newQuantity);
	Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null);
	ShoppingListDto GetShoppingList(Guid userId, Guid shoppingListId);
	IList<ShoppingListDto> GetNotClosedShoppingLists(Guid userId);
	void MarkProductAsNotPurchased(Guid userId, Guid list, Guid productGuid);
	void MarkProductAsPurchased(Guid userId, Guid shoppingListId, Guid productId);
	void MarkProductAsUnavailable(Guid userId, Guid shoppingListId, Guid productId);
	void RemoveProductFromList(Guid userId, Guid shoppingListId, Guid productId);
	Guid CreateNewListWithNotBoughtItems(Guid userId, Guid listId, Guid shopId2);
}