using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.persistance;

public interface IShoppingListRepository
{
	IList<ShoppingList> GetAll(Guid userId);
	ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid);
	void Save(ShoppingList shoppingList);
}

public class ShoppingListDao
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Guid { get; set; }
    public string? ShopId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Items { get; set; }
}
public class ShoppingListRepository : IShoppingListRepository
{
	public IList<ShoppingList> GetAll(Guid userId)
	{
		throw new NotImplementedException();
	}

	public ShoppingList GetShoppingList(Guid userId, Guid shoppingListGuid)
	{
		throw new NotImplementedException();
	}

	public void Save(ShoppingList shoppingList)
	{
		throw new NotImplementedException();
	}
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
			throw new ArgumentException("Not found");

		return cache[shoppingListGuid];
	}

	public void Save(ShoppingList shoppingList)
	{
		cache[shoppingList.Guid] = shoppingList;
	}
}