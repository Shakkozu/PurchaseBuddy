using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;

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

		foreach (var category in categoriesMap)
			if (!categories.Any(c => c.Guid == category))
				throw new ArgumentException($"category {category} not found");

		var userShop = UserShop.CreateNew(userId, userShopDescription, categories.Where(c => categoriesMap.Contains(c.Guid)).ToList());
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
			var userCategoriesMap = categories.Where(c => categoriesMap.Contains(c.Guid)).ToList();
			if (userCategoriesMap.Count != categoriesMap.Count)
				throw new Exception("not all categories were found");

			shop.ModifyShopConfiguration(userCategoriesMap);
		}

		userShopRepository.Save(shop);
	}

	public void DeleteUserShop(Guid userGuid, Guid shopId)
	{
		var shop = GetUserShopById(userGuid, shopId);
		if (shop == null)
			throw new ResourceNotFoundException("Shop not found");


		userShopRepository.Delete(shop);
	}
}
