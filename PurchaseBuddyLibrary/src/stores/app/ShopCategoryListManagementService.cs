using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.stores.persistance;

namespace PurchaseBuddyLibrary.src.stores.app;

public class ShopCategoryListManagementService : IShopCategoryListManagementService
{
	private readonly IUserShopRepository userShopRepository;
	private readonly IUserProductCategoriesRepository categoriesRepository;
	private readonly IShopMapRepository shopMapRepository;

	public ShopCategoryListManagementService(IUserShopRepository userShopRepository,
		IUserProductCategoriesRepository categoriesRepository,
		IShopMapRepository shopMapRepository)
	{
		this.userShopRepository = userShopRepository;
		this.categoriesRepository = categoriesRepository;
		this.shopMapRepository = shopMapRepository;
	}

	public void RemoveCategoryFromAllMaps(Guid userID, Guid categoryID)
	{
		var category = categoriesRepository.FindById(userID, categoryID);
		if (category is null)
			return;

		var maps = shopMapRepository.GetAll(userID);
		foreach (var map in maps)
		{
			map.RemoveCategory(category);
			shopMapRepository.Save(map);
		}
	}
	public void DefineNewCategoryMap(CreateOrUpdateCategoriesMapCommand command)
	{
		var shop = userShopRepository.GetUserShop(command.UserId, command.ShopId);
		if (shop is null)
			throw new ResourceNotFoundException($"Shop with id {command.ShopId} not found for user {command.UserId}");

		var userCategories = categoriesRepository.FindAll(command.UserId);
		if (!command.CategoriesMap.All(x => userCategories.Select(category => category.Guid).Contains(x)))
			throw new ArgumentException($"Not all categories exists");

		var shopMap = new ShopMap(command.UserId, command.ShopId, command.CategoriesMap);
		shopMapRepository.Save(shopMap);
	}

	public ShopMap? GetShopMap(Guid userId, Guid shopId)
	{
		return shopMapRepository.Get(userId, shopId);
	}

	public void UpdateExistingCategoryMap(CreateOrUpdateCategoriesMapCommand command)
	{
		var shopMap = shopMapRepository.Get(command.UserId, command.ShopId);
		if (shopMap is null)
			throw new ArgumentException($"Shop map does not exists");

		var shop = userShopRepository.GetUserShop(command.UserId, command.ShopId);
		if (shop is null)
			throw new ResourceNotFoundException($"Shop with id {command.ShopId} not found for user {command.UserId}");

		var userCategories = categoriesRepository.FindAll(command.UserId);
		if (!command.CategoriesMap.All(x => userCategories.Select(category => category.Guid).Contains(x)))
			throw new ArgumentException($"Not all categories exists");

		shopMap.UpdateCategoriesMap(command.CategoriesMap);
		shopMapRepository.Save(shopMap);
	}
}
