using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

namespace PurchaseBuddy.Tests.purchases.Integration;
internal class ShoppingListImportTests : PurchaseBuddyTestsFixture
{
    private IUserProductsManagementService productsManagementService;
    private IUserShopService shopService;
    private IUserProductCategoriesManagementService categoriesManagementService;
    private IShoppingListWriteService shoppingListWriteService;
    private IShoppingListReadService shoppingListReadService;

    private List<IProduct> products;
    private Guid shopId;
    private ShoppingListSharingFacade facade;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();
        PurchaseBuddyFixture.RegisterDependencies(services, TestConfigurationHelper.GetConnectionString());
        var serviceProvider = services.BuildServiceProvider();

        productsManagementService = serviceProvider.GetRequiredService<IUserProductsManagementService>();
        shopService = serviceProvider.GetRequiredService<IUserShopService>();
        categoriesManagementService = serviceProvider.GetRequiredService<IUserProductCategoriesManagementService>();
        shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();
        shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();
		facade = serviceProvider.GetRequiredService<ShoppingListSharingFacade>();
        Extensions.RecordElapsedTime("setup database", () =>
        {
            MigrationsRunner.ClearDatabase(services, TestConfigurationHelper.GetConnectionString());
            MigrationsRunner.RunMigrations(services, TestConfigurationHelper.GetConnectionString());
        });

        Extensions.RecordElapsedTime("Initialize", () =>
        {
            UserId = AUserCreated();
            InitializeTestContext();
        });
    }

	[Test]
	public void ShouldReturnAlreadyCreatedSharedList_WhenUserTriesToGenerateNewImportWithSameList()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var listToShareId = facade.CreateSharedList(UserId, listId);

		var secondCreatedList = facade.CreateSharedList(UserId, listId);

		Assert.AreEqual(listToShareId, secondCreatedList);
	}

	[Test]
	public void ShouldCreateNewSharedList_WhenSourceListWasModified()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var listToShareId = facade.CreateSharedList(UserId, listId);
		shoppingListWriteService.AddNewListItem(UserId, listId, new AddNewListItemRequest { ProductName ="Chicken breast"});

		var secondCreatedList = facade.CreateSharedList(UserId, listId);

		Assert.AreNotEqual(listToShareId, secondCreatedList);
	}

	[Test]
	public void ShouldKeepCategoryInformation_WhenListItemIsImported_WhenListIsShared()
	{
		var listItems = new List<ShoppingListItem> { ImportedShoppingListItem.CreateNew("Milk", "Dairy") };
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var sharedListId = facade.CreateSharedList(UserId, listId);

		var sharedList = facade.GetSharedList(sharedListId);

		Assert.AreEqual(sharedList.Items.First().categoryName, "Dairy");
	}

	[Test]
	public void ShouldKeepCategoryInformation_WhenListIsShared()
	{
		var category = categoriesManagementService.AddNewProductCategory(UserId, new CreateUserCategoryRequest("Dairy", null, null));
		var product = productsManagementService.DefineNewUserProduct(new UserProductDto { CategoryId = category, Guid = Guid.NewGuid(), Name = "Milk" }, UserId);
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var sharedListId = facade.CreateSharedList(UserId, listId);

		var sharedList = facade.GetSharedList(sharedListId);

		Assert.AreEqual(sharedList.Items.First().categoryName, "Dairy");
	}

	[Test]
    public void ShouldCreateSharedShoppingListFromExistingOne()
    {
        var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
        var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
        var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

        var listToShareId = facade.CreateSharedList(UserId, listId);

        var sharedList = facade.GetSharedList(listToShareId);
        var listItem = sharedList.Items.First();
        Assert.AreEqual(listItem.productName, "Milk");
    }

    [Test]
    public void ShouldThrowException_WhenExpectedShoppingListNotFound()
    {
		Assert.Throws<ArgumentException>(() => facade.GetSharedList(Guid.NewGuid()));
	}
    
    [Test]
    public void ShouldImportSharedListToDifferentUser()
    {
        var otherUserId = ANewUserCreated();
        var sharedListId = ASharedListCreated(UserId);
        
        facade.ImportSharedList(otherUserId, sharedListId);

        var shoppingList = shoppingListReadService.GetAllShoppingLists(otherUserId);
        Assert.That(shoppingList, Is.Not.Empty);
    }

    [Test]
    public void ShouldNotImportSharedListToCreator()
    {
        var sharedListId = ASharedListCreated(UserId);

        Assert.Throws<InvalidOperationException>(() => facade.ImportSharedList(UserId, sharedListId));
    }

    [Test]
    public void ShouldThrowException_WhenImportedListIsNotFound()
    {
        Assert.Throws<ArgumentException>(() => facade.ImportSharedList(UserId, Guid.NewGuid()));
    }

    private Guid ASharedListCreated(Guid userId)
    {
        var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
        var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
        var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
        var listToShareId = facade.CreateSharedList(UserId, listId);

        return listToShareId;
    }
    
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Extensions.RecordElapsedTime("Clear database", () =>
        {
            using (var connection = new NpgsqlConnection(TestConfigurationHelper.GetConnectionString()))
            {
                connection.Execute("delete from user_products");
                connection.Execute("delete from shared_products_customization");
                connection.Execute("delete from shared_products");
                connection.Execute("delete from shopping_lists");
                connection.Execute("delete from shops");
                connection.Execute("delete from product_categories_hierarchy");
                connection.Execute("delete from product_categories");
                connection.Execute("delete from users");
                connection.Close();
            }
        });
        _transactionScope.Dispose();
    }
    
    private void InitializeTestContext()
    {
        var product1 = productsManagementService.DefineNewUserProduct(UserProduct.Create("chicken breasts", UserId));
        var product2 = productsManagementService.DefineNewUserProduct(UserProduct.Create("milk", UserId));
        products = new[] { product1, product2 }.ToList();

        var category1 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
        var category2 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
        var category3 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());

        shopId = shopService.AddNew(UserId, UserShopDescription.CreateNew("test1"), new List<Guid> { category1, category2 });
    }
}