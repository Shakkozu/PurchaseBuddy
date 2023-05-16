using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddy.Tests.catalogue;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;

namespace PurchaseBuddy.Tests.stores.integration;
internal class UserShopServiceTests : CatalogueTestsFixture
{
	private IUserShopRepository userShopRepository;
	private UserShopService userShopService;
	private IUserProductCategoriesManagementService categoriesManagementService;

	[SetUp]
	public void SetUp()
	{
		userShopRepository = new ShopsRepository(TestConfigurationHelper.GetConnectionString());
		categoriesManagementService = new UserProductCategoriesManagementService(new InMemoryUserProductCategoriesRepository(), new InMemoryProductsRepository());
		userShopService = new UserShopService(userShopRepository, categoriesManagementService);
		TearDown();
		UserId = AUserCreated();
	}

	[TearDown]
	public override void TearDown()
	{
		base.TearDown();

	}

	[Test]
	public void GetUserShop_AssertCorrectIsReturned()
	{
		var addedShopId = userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"));

		Assert.NotNull(userShopService.GetUserShopById(UserId, addedShopId));
	}
	
	[Test]
	public void DisabledShopsShouldNotBeReturned()
	{
		var addedShopId = userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"));

		userShopService.DeleteUserShop(UserId, addedShopId);

		Assert.Null(userShopService.GetUserShopById(UserId, addedShopId));
	}

	[Test]
	public void GetAllUserShops_AssertAllUserShopsAreReturned()
	{
		userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"));
		userShopService.AddNew(UserId, UserShopDescription.CreateNew("test2"));

		Assert.AreEqual(2, userShopService.GetAllUserShops(UserId).Count);
	}
	
	[Test]
	public void UpdateUserShopCategoriesMap_WhenCategoriesAreNested()
	{
		var parent = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));
		var child = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, parent));
		var shop = userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"));

		userShopService.Update(UserShopDescription.CreateNew("test"), UserId, shop, new List<Guid> { parent, child });

		var usershop = userShopService.GetUserShopById(UserId, shop);
		Assert.AreEqual(2, usershop.ConfigurationEntries.Count);
		Assert.AreEqual(parent, usershop.ConfigurationEntries.First().CategoryGuid);
		Assert.AreEqual(child, usershop.ConfigurationEntries.Last().CategoryGuid);
	}

	[Test]
	public void WhenShopWithConfigurationIsCreated_AssertCategoriesAreSavedInValidOrder()
	{
		var cat1 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));
		var cat2 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));
		var cat3 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));

		var shop = userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"), new[] {cat3, cat2, cat1}.ToList());

		var usershop = userShopService.GetUserShopById(UserId, shop);
		Assert.AreEqual(cat3, usershop.ConfigurationEntries[0].CategoryGuid);
		Assert.AreEqual(cat2, usershop.ConfigurationEntries[1].CategoryGuid);
		Assert.AreEqual(cat1, usershop.ConfigurationEntries[2].CategoryGuid);
	}

	[Test]
	public void WhenShopWithConfigurationIsUpdated_AssertCategoriesAreSavedInValidOrder()
	{
		var cat1 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));
		var cat2 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));
		var cat3 = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("test", null, null));

		var shop = userShopService.AddNew(UserId, UserShopDescription.CreateNew("test"), new[] {cat1, cat2, cat3}.ToList());
		userShopService.Update(AUserShopDescription(), UserId, shop, new[] { cat3, cat2, cat1 }.ToList());

		var usershop = userShopService.GetUserShopById(UserId, shop);
		Assert.AreEqual(cat3, usershop.ConfigurationEntries[0].CategoryGuid);
		Assert.AreEqual(cat2, usershop.ConfigurationEntries[1].CategoryGuid);
		Assert.AreEqual(cat1, usershop.ConfigurationEntries[2].CategoryGuid);
	}

	private UserShopDescription AUserShopDescription()
	{
		return UserShopDescription.CreateNew("blabla");
	}
}
