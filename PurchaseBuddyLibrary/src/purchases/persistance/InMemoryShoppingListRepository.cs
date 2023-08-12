using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.persistance;

public class InMemoryShoppingListRepository : IShoppingListRepository
{
	private Dictionary<Guid, ShoppingList> cache = new();

	public IList<ShoppingList> GetAll(Guid userId)
	{
		return cache.Values
			.Where(list => list.UserId == userId || list.UsersAllowedToModify.Contains(userId))
			.ToList();
	}

	public ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid)
	{
		if (!cache.ContainsKey(shoppingListGuid))
			return null;

		var list = cache[shoppingListGuid];
		if (list.UserId != userId && !list.UsersAllowedToModify.Contains(userId))
			return null;


		return cache[shoppingListGuid];
	}

	public ShoppingList? GetShoppingList(Guid shoppingListGuid)
	{
		if (!cache.ContainsKey(shoppingListGuid))
			return null;

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
