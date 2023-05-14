using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.stores.persistance;

namespace PurchaseBuddyLibrary.src.stores.app;

public class ShopCategoryListManagementService : IShopCategoryListManagementService
{
	private readonly IUserShopRepository userShopRepository;
	private readonly IUserProductCategoriesRepository categoriesRepository;

	public ShopCategoryListManagementService(IUserShopRepository userShopRepository,
		IUserProductCategoriesRepository categoriesRepository)
	{
		this.userShopRepository = userShopRepository;
		this.categoriesRepository = categoriesRepository;
	}

	public void RemoveCategoryFromAllMaps(Guid userID, Guid categoryID)
	{
		var category = categoriesRepository.FindById(userID, categoryID);
		if (category is null)
			return;

		var shops = userShopRepository.GetAllUserShops(userID);
		for (var i = 0; i < shops.Count; i++)
		{
			var shop = shops[i];
			shop.RemoveCategoryFromConfiguration(category);
			userShopRepository.Save(shop);
		}
	}
}
