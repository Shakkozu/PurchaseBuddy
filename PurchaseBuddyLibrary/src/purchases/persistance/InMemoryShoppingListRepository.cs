using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.purchases.contract;

namespace PurchaseBuddy.src.purchases.persistance;

public interface IShoppingListRepository
{
	IList<ShoppingList> GetAll(Guid userId);
	ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid);
	void Save(ShoppingList shoppingList);
}
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
			return null;

		return cache[shoppingListGuid];
	}

	public void Save(ShoppingList shoppingList)
	{
		cache[shoppingList.Guid] = shoppingList;
	}
}