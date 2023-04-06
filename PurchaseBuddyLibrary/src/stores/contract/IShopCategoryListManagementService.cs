namespace PurchaseBuddyLibrary.src.stores.app;

public interface IShopCategoryListManagementService
{
	void DefineNewCategoryMap(CreateOrUpdateCategoriesMapCommand command);
	ShopMap? GetShopMap(Guid userId, Guid shopId);
	void RemoveCategoryFromAllMaps(Guid userID, Guid categoryID);
	void UpdateExistingCategoryMap(CreateOrUpdateCategoriesMapCommand command);
}
