using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.persistance;

public interface IShoppingListRepository
{
	ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid);
	void Save(ShoppingList shoppingList);
}
public class InMemoryShoppingListRepository : IShoppingListRepository
{
	private Dictionary<Guid, ShoppingList> cache = new();
	public ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid)
	{
		if (!cache.ContainsKey(shoppingListGuid))
			return null;

		return cache[shoppingListGuid];
	}

	public void Save(ShoppingList shoppingList)
	{
		cache[shoppingList.Guid] = shoppingList;
	}
}