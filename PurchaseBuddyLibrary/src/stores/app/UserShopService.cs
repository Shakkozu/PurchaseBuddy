using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.stores.app;

public class UserShopService : IUserShopService
{
	private readonly IUserShopRepository userShopRepository;
	private readonly IUserProductCategoriesManagementService categoriesManagementService;

	public UserShopService(IUserShopRepository userShopRepository,
		IUserProductCategoriesManagementService categoriesManagementService)
	{
		this.userShopRepository = userShopRepository;
		this.categoriesManagementService = categoriesManagementService;
	}

	public Guid AddNew(Guid userId, UserShopDescription userShopDescription, List<Guid>? categoriesMap = null)
	{
		var categories = categoriesManagementService.GetCategoriesAsFlatList(userId);
		if(categoriesMap == null)
			categoriesMap = new List<Guid>();

		var userShop = UserShop.CreateNew(userId, userShopDescription, GetNewCategoryMap(categoriesMap, categories));
		userShopRepository.Save(userShop);

		return userShop.Guid;
	}

	public UserShop? GetUserShopById(Guid userId, Guid userShopId)
	{
		return userShopRepository.GetUserShop(userId, userShopId);
	}

	public List<UserShop> GetAllUserShops(Guid userId)
	{
		return userShopRepository.GetAllUserShops(userId);
	}

	public void Update(UserShopDescription userShopDescription, Guid userGuid, Guid shopId, List<Guid>? categoriesMap = null)
	{
		var shop = GetUserShopById(userGuid, shopId);
		if (shop == null)
			throw new ResourceNotFoundException("Shop not found");

		shop.ChangeDescriptionTo(userShopDescription);
		if(categoriesMap != null && categoriesMap.Any())
		{
			var categories = categoriesManagementService.GetCategoriesAsFlatList(userGuid);
			shop.ModifyShopConfiguration(GetNewCategoryMap(categoriesMap, categories));
		}

		userShopRepository.Save(shop);
	}

	private static List<IProductCategory> GetNewCategoryMap(List<Guid>? categoriesMap, IEnumerable<IProductCategory> categories)
	{
		var result = new List<IProductCategory>();
		if (categoriesMap == null || !categoriesMap.Any())
			return result;

		categoriesMap.ForEach(categoryGuid =>
		{
			var category = categories.FirstOrDefault(cat => cat.Guid == categoryGuid);
			if (category == null)
				throw new Exception($"Category with guid {categoryGuid} was not found");

			result.Add(category);
		});
		return result;
	}

	public void DeleteUserShop(Guid userGuid, Guid shopId)
	{
		var shop = GetUserShopById(userGuid, shopId);
		if (shop == null)
			throw new ResourceNotFoundException("Shop not found");


		userShopRepository.Delete(shop);
	}
}
