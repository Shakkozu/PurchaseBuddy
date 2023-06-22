using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddy.src.purchases.app;

public class ShoppingListWriteService : IShoppingListWriteService
{
	private readonly IShoppingListRepository shoppingListRepository;

	public ShoppingListWriteService(IShoppingListRepository shoppingListRepository)
    {
		this.shoppingListRepository = shoppingListRepository;
	}

	public void UpdateProductsOnList(Guid userId, Guid listId, List<Guid> productsIDs)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, listId);
		if (shoppingList == null)
			throw new ArgumentException($"Shopping list with id {listId} not found");

		var result = productsIDs.Select(productId => ShoppingListItem.CreateNew(productId));
		shoppingList.UpdateListItems(result);
		shoppingListRepository.Update(shoppingList);

	}
	public void MarkProductAsUnavailable(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsUnavailable(productId);
		shoppingListRepository.Update(shoppingList);
	}
	public void MarkProductAsPurchased(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsPurchased(productId);
		shoppingListRepository.Update(shoppingList);
	}
	public void MarkProductAsNotPurchased(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsNotPurchased(productId);
		shoppingListRepository.Update(shoppingList);
	}
	public Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null)
	{
		var list = ShoppingList.CreateNew(userId, listItems, assignedShop);
		shoppingListRepository.Save(list);

		return list.Guid;
	}
	public void RemoveProductFromList(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.Remove(productId);
		shoppingListRepository.Update(shoppingList);
	}
	public void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid productId, int newQuantity)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.ChangeQuantityOf(productId, newQuantity);
		shoppingListRepository.Update(shoppingList);
	}

	public Guid CreateNewListWithNotBoughtItems(Guid userId, Guid shoppingListId, Guid shopId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		var newShoppingList = shoppingList.GenerateNewWithNotBoughtItems(shopId);
		shoppingListRepository.Update(shoppingList);
		shoppingListRepository.Save(newShoppingList);

		return newShoppingList.Guid;
	}
}
