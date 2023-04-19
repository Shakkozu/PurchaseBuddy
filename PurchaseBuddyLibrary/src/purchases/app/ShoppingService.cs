using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddy.src.purchases.app;
public class ShoppingService
{
	private readonly IShoppingListRepository shoppingListRepository;

	public ShoppingService(IShoppingListRepository shoppingListRepository)
	{
		this.shoppingListRepository = shoppingListRepository;
	}

	public Guid CreateNewShoppingList(Guid userId, Guid? shopId = null)
	{
		var shoppingList = ShoppingList.CreateNew(userId, shopId);
		shoppingListRepository.Save(shoppingList);

		return shoppingList.Guid;
	}

	public void AddProductToShoppingList(Guid shoppingListId, Guid userId, Guid productId, int quantity = 1)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList is null)
			throw new ArgumentException($"There is no shopping list with id: {shoppingListId}");

		shoppingList.AddNew(new ShoppingListItem(productId, quantity));
		shoppingListRepository.Save(shoppingList);
	}

	public void UpdateProductQuantityOnShoppingList(Guid shoppingListId, Guid userId, Guid productId, int newQuantity)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList is null)
			throw new ArgumentException($"There is no shopping list with id: {shoppingListId}");

		shoppingList.ChangeQuantityOf(productId, newQuantity);
		shoppingListRepository.Save(shoppingList);
	}

	public void DeleteProductFromShoppingList(Guid shoppingListId, Guid userId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList is null)
			throw new ArgumentException($"There is no shopping list with id: {shoppingListId}");

		shoppingList.Remove(productId);
		shoppingListRepository.Save(shoppingList);
	}

	public void MarkShoppingListAsClosed(Guid userId, Guid shoppingListId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList is null)
			throw new ArgumentException($"There is no shopping list with id: {shoppingListId}");

		shoppingList.Complete();
		shoppingListRepository.Save(shoppingList);
	}
}
