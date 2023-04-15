using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddyLibrary.src.stores.app;

public interface IShopCategoryListManagementService
{
	void RemoveCategoryFromAllMaps(Guid userID, Guid categoryID);
}
