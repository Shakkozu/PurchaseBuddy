using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.stores.app;

namespace PurchaseBuddy.Tests.stores.categories_maps;

internal class ShopCategoryListManagementServiceTests
{
	private ShopCategoryListManagementService categoriesMapService;
	private InMemoryUserShopRepository userShopRepository;
	private UserShopService userShopService;
	private UserProductCategoriesManagementService userProductCategoriesService;

	[SetUp]
	public void SetUp()
	{
		userShopRepository = new InMemoryUserShopRepository();
		userShopService = new UserShopService(userShopRepository);
		var userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		var userProductsRepo = new InMemoryProductsRepository();
		var shopListService = new ShopCategoryListManagementService(new InMemoryUserShopRepository(), new InMemoryUserProductCategoriesRepository(), new InMemoryShopMapRepository());
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		var shopMapRepo = new InMemoryShopMapRepository();
		categoriesMapService = new ShopCategoryListManagementService(userShopRepository, userCategoriesRepo, shopMapRepo);
	}

	[Test]
	public void UserCanDefineNewCategoriesMap()
	{
		var productCategories = AProductCategories(2);
		var request = ACreateCategoryMapInDescendingOrderRequest(AShop(), productCategories);

		categoriesMapService.DefineNewCategoryMap(request);

		AssertThatCategoryThatMatchesRequestExists(request);
	}
	
	[Test]
	public void UserCanModifyExistingCategoriesMap()
	{
		var shop = AShop();
		categoriesMapService.DefineNewCategoryMap(ACreateCategoryMapInDescendingOrderRequest(shop, AProductCategories(2)));
		var request = ACreateCategoryMapInDescendingOrderRequest(shop, AProductCategories(5));

		categoriesMapService.UpdateExistingCategoryMap(request);

		AssertThatCategoryThatMatchesRequestExists(request);
	}

	[Test]
	public void SameCategoryCannotBeAddedTwiceToCategoryList()
	{
		var shop = AShop();
		var productCategories = AProductCategories(1);
		var request = ACreateCategoryMapInDescendingOrderRequest(shop, new List<Guid> { productCategories.First(), productCategories.First() });

		categoriesMapService.DefineNewCategoryMap(request);

		var shopCategoryMap = categoriesMapService.GetShopMap(Fixture.UserId, shop);
		Assert.AreEqual(1, shopCategoryMap.Categories.Count);
	}

	private List<Guid> AProductCategories(int n)
	{
		var result = new List<Guid>();
		for (var i = 0; i < n; i++)
			result.Add(userProductCategoriesService.AddNewProductCategory(Fixture.UserId, new CreateUserCategoryRequest("testCategory" + i, null, null)));

		return result;
	}

	private Guid AShop()
	{
		return userShopService.AddNewUserShop(Fixture.UserId, UserShopDescription.CreateNew("test shop"));
	}

	private void AssertThatCategoryThatMatchesRequestExists(CreateOrUpdateCategoriesMapCommand command)
	{
		var shopCategoryMap = categoriesMapService.GetShopMap(command.UserId, command.ShopId);

		Assert.That(shopCategoryMap, Is.Not.Null);
		Assert.AreEqual(command.CategoriesMap.Count, shopCategoryMap.Categories.Count);
		for(int i = 0; i < command.CategoriesMap.Count; i++)
			Assert.AreEqual(shopCategoryMap.Categories[i], command.CategoriesMap[i]);
	}

	private CreateOrUpdateCategoriesMapCommand ACreateCategoryMapInDescendingOrderRequest(Guid shopId, List<Guid> productCategories)
	{
		return new CreateOrUpdateCategoriesMapCommand
		{
			UserId = Fixture.UserId,
			ShopId = shopId,
			CategoriesMap = productCategories
		};
	}
}
