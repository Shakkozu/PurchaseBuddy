using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.src.stores.persistance;

public interface IUserShopRepository
{
	void Save(UserShop userShop);
	List<UserShop> GetAllUserShops(Guid userId);
	UserShop? GetUserShop(Guid userId, Guid userShopId);
	void Delete(UserShop shop);
	void Update(UserShop shop);
}
