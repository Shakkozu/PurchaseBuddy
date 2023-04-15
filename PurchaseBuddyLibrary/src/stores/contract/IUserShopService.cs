using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.src.stores.app;

public interface IUserShopService
{
	Guid AddNew(Guid userId, UserShopDescription userShopDescription, List<Guid>? categoriesMap = null);
	void DeleteUserShop(Guid userGuid, Guid shopId);
	List<UserShop> GetAllUserShops(Guid userId);
	UserShop? GetUserShopById(Guid userId, Guid userShopId);
	void Update(UserShopDescription userShopDescription, Guid userGuid, Guid shopId, List<Guid>? categoriesMap = null);
}
