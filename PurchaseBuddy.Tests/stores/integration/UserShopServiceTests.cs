using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.catalogue.contract;

namespace PurchaseBuddy.Tests.stores.integration;
internal class UserShopServiceTests
{
	private InMemoryUserShopRepository userShopRepository;
	private UserShopService userShopService;
	private IUserProductCategoriesManagementService categoriesManagementService;

	[SetUp]
	public void SetUp()
	{
		userShopRepository = new InMemoryUserShopRepository();
		categoriesManagementService = new UserProductCategoriesManagementService(new InMemoryUserProductCategoriesRepository(), new InMemoryProductsRepository());
		userShopService = new UserShopService(userShopRepository, categoriesManagementService);
	}

	[Test]
	public void GetUserShop_AssertCorrectIsReturned()
	{
		var addedShopId = userShopService.AddNew(Fixture.UserId, UserShopDescription.CreateNew("test"));

		Assert.NotNull(userShopService.GetUserShopById(Fixture.UserId, addedShopId));
	}

	[Test]
	public void GetAllUserShops_AssertAllUserShopsAreReturned()
	{
		userShopService.AddNew(Fixture.UserId, UserShopDescription.CreateNew("test"));
		userShopService.AddNew(Fixture.UserId, UserShopDescription.CreateNew("test2"));

		Assert.AreEqual(2, userShopService.GetAllUserShops(Fixture.UserId).Count);
	}
	
	[Test]
	public void UpdateUserShopCategoriesMap_WhenCategoriesAreNested()
	{
		var parent = categoriesManagementService.AddNewProductCategory(Fixture.UserId, new CreateUserCategoryRequest("test", null, null));
		var child = categoriesManagementService.AddNewProductCategory(Fixture.UserId, new CreateUserCategoryRequest("test", null, parent));
		var shop = userShopService.AddNew(Fixture.UserId, UserShopDescription.CreateNew("test"));

		userShopService.Update(UserShopDescription.CreateNew("test"), Fixture.UserId, shop, new List<Guid> { parent, child });

		var usershop = userShopService.GetUserShopById(Fixture.UserId, shop);
		Assert.AreEqual(2, usershop.ConfigurationEntries.Count);
		Assert.AreEqual(parent, usershop.ConfigurationEntries.First().CategoryGuid);
		Assert.AreEqual(child, usershop.ConfigurationEntries.Last().CategoryGuid);
	}
}
