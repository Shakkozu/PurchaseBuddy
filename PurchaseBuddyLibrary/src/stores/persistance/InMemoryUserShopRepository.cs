using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.src.stores.persistance;

public class InMemoryUserShopRepository : IUserShopRepository
{
	private Dictionary<Guid, List<UserShop>> cache = new();

	public void Delete(UserShop shop)
	{
		if (cache.TryGetValue(shop.UserId, out var userShops))
			userShops.Remove(shop);
	}

	public List<UserShop> GetAllUserShops(Guid userId)
	{
		if (cache.ContainsKey(userId))
			return cache[userId];

		return new List<UserShop>();
	}

	public UserShop? GetUserShop(Guid userId, Guid userShopId)
	{
		if (cache.ContainsKey(userId))
			return cache[userId].FirstOrDefault(shop => shop.Guid == userShopId);

		return null;
	}

	public void Save(UserShop userShop)
	{
		if (!cache.ContainsKey(userShop.UserId))
		{
			cache[userShop.UserId] = new List<UserShop> { userShop };
			return;
		}

		var modifiedShop = cache[userShop.UserId].FirstOrDefault(shop => shop.Guid == userShop.Guid);
		if (modifiedShop != null)
		{
			cache[userShop.UserId].Remove(modifiedShop);
			cache[userShop.UserId].Add(userShop);
			return;
		}

		cache[userShop.UserId].Add(userShop);
	}

	public void Update(UserShop shop)
	{
		var shopToUpdate = cache[shop.UserId].FirstOrDefault(userShop => shop.Guid == userShop.Guid);
		if (shopToUpdate == null)
			return;

		cache[shop.UserId].Remove(shopToUpdate);
		cache[shop.UserId].Add(shop);
    }
}
