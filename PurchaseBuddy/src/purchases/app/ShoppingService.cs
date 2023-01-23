using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddy.src.purchases.app;
public class ShoppingService
{
	private readonly IShoppingListRepository shoppingListRepository;

	public ShoppingService()
	{
	}

	public ShoppingService(IShoppingListRepository shoppingListRepository)
	{
		this.shoppingListRepository = shoppingListRepository;
	}

	public Guid CreateNewShoppingList(Guid userId)
	{
		var shoppingList = ShoppingList.CreateNew(userId);
		shoppingListRepository.Save(shoppingList);

		return shoppingList.Guid;
	}

	public void MarkShoppingListAsClosed(Guid userId, Guid shoppingListId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList is null)
			throw new ArgumentException($"There is no shopping list with id: {shoppingListId}");

		shoppingList.Close();
		shoppingListRepository.Save(shoppingList);
	}
}
