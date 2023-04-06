using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.src.stores.app;

public interface IUserShopService
{
	Guid AddNewUserShop(Guid userId, UserShopDescription userShopDescription);
	void DeleteUserShop(Guid userGuid, Guid shopId);
	List<UserShop> GetAllUserShops(Guid userId);
	UserShop? GetUserShopById(Guid userId, Guid userShopId);
	void UpdateShopDescription(UserShopDescription userShopDescription, Guid userGuid, Guid shopId);
}
