using PurchaseBuddyLibrary.src.stores.persistance;

namespace PurchaseBuddyLibrary.src.stores.app;

public class InMemoryShopMapRepository : IShopMapRepository
{
	private readonly List<ShopMap> cache = new();

	public void Save(ShopMap shopMap)
	{
		var existing = Get(shopMap.UserId, shopMap.ShopId);
		if(existing is not null)
			cache.Remove(existing);

		cache.Add(shopMap);
	}

	public ShopMap? Get(Guid userId, Guid shopId)
	{
		return cache.FirstOrDefault(sm => sm.UserId == userId && sm.ShopId == shopId);
	}

	public List<ShopMap> GetAll(Guid userID)
	{
		return cache.Where(map => map.UserId == userID).ToList();
	}
}
