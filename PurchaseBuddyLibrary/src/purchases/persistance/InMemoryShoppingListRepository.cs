using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.persistance;

public class InMemoryShoppingListRepository : IShoppingListRepository
{
	private Dictionary<Guid, ShoppingList> cache = new();

	public IList<ShoppingList> GetAll(Guid userId)
	{
		return cache.Values
			.Where(list => list.UserId == userId)
			.ToList();
	}

	public ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid)
	{
		if (!cache.ContainsKey(shoppingListGuid))
			throw new ArgumentException("Not found");

		return cache[shoppingListGuid];
	}

	public void Save(ShoppingList shoppingList)
	{
		cache[shoppingList.Guid] = shoppingList;
	}

	public void Update(ShoppingList shoppingList)
	{
		cache[shoppingList.Guid] = shoppingList;
	}
}
