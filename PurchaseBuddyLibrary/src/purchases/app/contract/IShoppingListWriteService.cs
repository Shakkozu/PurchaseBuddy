using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;

public interface IShoppingListWriteService
{
	void UpdateProductsOnList(Guid userId, Guid shoppingListId, List<Guid> productsIDs);
	void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid productId, int newQuantity);
	void MarkProductAsNotPurchased(Guid userId, Guid list, Guid productGuid);
	void MarkProductAsPurchased(Guid userId, Guid shoppingListId, Guid productId);
	void MarkProductAsUnavailable(Guid userId, Guid shoppingListId, Guid productId);
	void RemoveProductFromList(Guid userId, Guid shoppingListId, Guid productId);
	Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null);
	Guid CreateNewListWithNotBoughtItems(Guid userId, Guid listId, Guid shopId2);
}
