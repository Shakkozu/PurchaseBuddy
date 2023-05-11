using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.purchases.Integration;

internal class ShoppingListServiceTests : PurchaseBuddyTestsFixture
{
	private IUserProductsManagementService productsManagementService;
	private IUserShopService shopService;
	private IUserProductCategoriesManagementService categoriesManagementService;
	private IShoppingListService shoppingListProductsManagementService;

	[SetUp]
	public void SetUp()
	{
		var services = new ServiceCollection();
		var configBuilder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		var configuration = configBuilder.Build();
		var databaseConnectionString = configuration.GetConnectionString("Database");
		PurchaseBuddyFixture.RegisterDependencies(services, databaseConnectionString);

		var serviceProvider = services.BuildServiceProvider();
		productsManagementService = serviceProvider.GetRequiredService<IUserProductsManagementService>();
		shopService = serviceProvider.GetRequiredService<IUserShopService>();
		categoriesManagementService = serviceProvider.GetRequiredService<IUserProductCategoriesManagementService>();
		shoppingListProductsManagementService = serviceProvider.GetRequiredService<IShoppingListService>();
		UserId = AUserCreated();
		InitializeTestContext();
	}

	[TearDown]
	public void TearDown()
	{
		using (var connection = new NpgsqlConnection(TestConfigurationHelper.GetConnectionString()))
		{
			connection.Execute("delete from product_categories_hierarchy");
			connection.Execute("delete from product_categories");
			connection.Execute("delete from users");
		}
	}

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var servicesCollection = new ServiceCollection();
		MigrationsRunner.ClearDatabase(servicesCollection, TestConfigurationHelper.GetConnectionString());
		MigrationsRunner.RunMigrations(servicesCollection, TestConfigurationHelper.GetConnectionString());
	}

	[Test]
	public void ShouldOrderShoppingListItemsByAssignedShopCategoriesConfig()
	{
		Guid category1 = categoriesGuids[2], category2 = categoriesGuids[0], category3 = categoriesGuids[1];
		var productCat1 = AProductWithCategory(category1);
		var productCat2 = AProductWithCategory(category2);
		var productCat3 = AProductWithCategory(category3);
		var shop = AShopWithCategories(new[] { category1, category2, category3 });
		var listItems = AListItems(new[] {productCat1, productCat2, productCat3});
		var list = shoppingListProductsManagementService.CreateNewList(UserId, listItems, shop);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, list);

		Assert.AreEqual(productCat1, userList.ShoppingListItems[0].ProductDto.Guid);
		Assert.AreEqual(productCat2, userList.ShoppingListItems[1].ProductDto.Guid);
		Assert.AreEqual(productCat3, userList.ShoppingListItems[2].ProductDto.Guid);
	}
	
	[Test]
	public void ShouldSaveShoppingListsFieldsCorrectly()
	{
		var listItems = AListItems();
		var list = shoppingListProductsManagementService.CreateNewList(UserId, listItems, shopId);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, list);

		Assert.AreEqual(shopId, userList.AssignedShop?.Guid);
		Assert.AreEqual(userList.ShoppingListItems.First().ProductDto.Guid, listItems.First().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.First().Quantity, listItems.First().Quantity);
		Assert.AreEqual(userList.ShoppingListItems.Last().ProductDto.Guid, listItems.Last().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.Last().Quantity, listItems.Last().Quantity);
	}
	
	[Test]
	public void ShouldReturnAllNotCompletedLists()
	{
		var listItems = AListItemsWithSingleItem();
		var list = shoppingListProductsManagementService.CreateNewList(UserId, listItems, shopId);
		var list2 = shoppingListProductsManagementService.CreateNewList(UserId, listItems, shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, list, listItems.First().ProductId);
		var userLists = shoppingListProductsManagementService.GetNotClosedShoppingLists(UserId);

		Assert.AreEqual(1, userLists.Count);
		Assert.AreEqual(list2, userLists.First().Guid);
	}
	
	[Test]
	public void ShouldNotCreateEmptyList()
	{
		var listItems = new List<ShoppingListItem> { };

		Assert.Throws<InvalidOperationException>(() => shoppingListProductsManagementService.CreateNewList(UserId, listItems, shopId));
	}

	[Test]
	public void ShouldMarkListItemAsPurchased()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, list, productGuid);

		var listItem = shoppingListProductsManagementService.GetShoppingList(UserId, list).ShoppingListItems.First(li => li.ProductDto.Guid == productGuid);
		Assert.True(listItem.Purchased);
	}

	[Test]
	public void ShouldMarkListItemAsNotPurchased()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, list, productGuid);
		shoppingListProductsManagementService.MarkProductAsNotPurchased(UserId, list, productGuid);

		var listItem = shoppingListProductsManagementService.GetShoppingList(UserId, list).ShoppingListItems.First(li => li.ProductDto.Guid == productGuid);
		Assert.False(listItem.Purchased);
	}

	[Test]
	public void ShouldContainInformationAboutAssignedShop()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, list, productGuid);
		shoppingListProductsManagementService.MarkProductAsNotPurchased(UserId, list, productGuid);

		var listItem = shoppingListProductsManagementService.GetShoppingList(UserId, list).ShoppingListItems.First(li => li.ProductDto.Guid == productGuid);
		Assert.False(listItem.Purchased);
	}

	[Test]
	public void ShouldContainInformationsAboutAssignedShop()
	{
		var list = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, list);

		Assert.NotNull(userList.AssignedShop.Name);
	}


	[Test]
	public void ShouldChangeProductQuantity()
	{
		var productGuid = products.First().Guid;
		var newQuantity = 10;
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.ChangeQuantityOfProductOnList(UserId, listId, productGuid, 10);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.AreEqual(newQuantity, userList.ShoppingListItems.First(p => p.ProductDto.Guid == productGuid).Quantity);
	}


	[Test]
	public void ShouldMarkProductAsUnavailable()
	{
		var productGuid = products.First().Guid;
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.MarkProductAsUnavailable(UserId, listId, productGuid);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.True(userList.ShoppingListItems.First(p => p.ProductDto.Guid == productGuid).Unavailable);
	}

	[Test]
	public void ShouldCreateNewListWithNotPurchasedItems()
	{
		var productGuid = products.First().Guid;
		var listItems = AListItems();
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, listItems, shopId);

		shoppingListProductsManagementService.MarkProductAsUnavailable(UserId, listId, productGuid);
		var newListId = shoppingListProductsManagementService.CreateNewListWithNotBoughtItems(UserId, listId, shopId2);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, newListId);
		Assert.AreEqual(shopId2, userList.AssignedShop.Guid);
		Assert.AreEqual(userList.ShoppingListItems.First().ProductDto.Guid, listItems.First().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.First().Quantity, listItems.First().Quantity);
		Assert.AreEqual(userList.ShoppingListItems.Last().ProductDto.Guid, listItems.Last().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.Last().Quantity, listItems.Last().Quantity);
	}

	[Test]
	public void ShouldNotCreateNewListWithNotPurchasedItems_WhenAllItemsArePurchased()
	{
		var productGuid = products.First().Guid;
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItemsWithSingleItem(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, listId, productGuid);

		Assert.Throws<InvalidOperationException>(() => shoppingListProductsManagementService.CreateNewListWithNotBoughtItems(UserId, listId, shopId2));
	}

    [Test]
    public void ShouldCreateNewListWithNotPurchasedItemsAndCompleteCurrentList()
    {
		var productGuid = products.First().Guid;
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
	}
	

    [Test]
    public void ShouldCreateNewListWithNotPurchasedItemsAndRemoveNotCompletedItems()
    {
		var productGuid = products.First().Guid;
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.IsEmpty(userList.ShoppingListItems);
	}

    [Test]
    public void ShouldMarkListAsCompletedWhenLastProductIsPurchased()
    {
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItemsWithSingleItem(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, listId, products.First().Guid);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

    [Test]
    public void ShouldCompleteListWhenLastNotPurchasedProductIsRemoved()
    {
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, listId, products.First().Guid);
		shoppingListProductsManagementService.RemoveProductFromList(UserId, listId, products.Last().Guid);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

    [Test]
    public void CannotAddProductToCompletedList()
    {
		var listId = shoppingListProductsManagementService.CreateNewList(UserId, AListItemsWithSingleItem(), shopId);

		shoppingListProductsManagementService.MarkProductAsPurchased(UserId, listId, products.First().Guid);
		
		Assert.Throws<InvalidOperationException>(() => shoppingListProductsManagementService.AddProductToList(UserId, listId, UserProduct.Create("blabla", UserId)));
	}

    [Test]
    public void ShouldRemoveProductFromList()
    {
        var list = shoppingListProductsManagementService.CreateNewList(UserId, AListItems(), shopId);

        shoppingListProductsManagementService.RemoveProductFromList(UserId, list, products.First().Guid);

		var userList = shoppingListProductsManagementService.GetShoppingList(UserId, list);
		Assert.AreEqual(1, userList.ShoppingListItems.Count);
	}

    private List<ShoppingListItem> AListItems(IEnumerable<Guid> _products = null)
    {
		if(_products != null)
			return _products.Select(product => new ShoppingListItem(product)).ToList();

        return new List<ShoppingListItem>
        {
            new ShoppingListItem(products.First().Guid, 1),
            new ShoppingListItem(products.Last().Guid, 2),
        };
    }

    private List<ShoppingListItem> AListItemsWithSingleItem()
    {
        return new List<ShoppingListItem>
        {
            new ShoppingListItem(products.First().Guid, 1),
        };
    }

    private void InitializeTestContext()
    {
        var product1 = productsManagementService.DefineNewUserProduct(UserProduct.Create("chicken breasts", UserId));
        var product2 = productsManagementService.DefineNewUserProduct(UserProduct.Create("milk", UserId));
        products = new[] { product1, product2 }.ToList();

        var category1 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
        var category2 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
        var category3 = categoriesManagementService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
        categoriesGuids = new[] { category1, category2, category3 }.ToList();

        shopId = shopService.AddNew(UserId, UserShopDescription.CreateNew("test1"), new List<Guid> { category1, category2 });
        shopId2 = shopService.AddNew(UserId, UserShopDescription.CreateNew("test2"), new List<Guid> { category2, category1 });
    }

	private Guid AProductWithCategory(Guid categoryGuid)
	{
		return productsManagementService.DefineNewUserProduct(UserProduct.Create("test name", UserId, categoryGuid)).Guid;
	}

	private Guid AShopWithCategories(IEnumerable<Guid> categories)
	{
		return shopService.AddNew(UserId, UserShopDescription.CreateNew("test"), categories.ToList());
	}
	private Guid AUserCreated()
	{
		var userRepository = new UserRepository(TestConfigurationHelper.GetConnectionString());
		var authService = new AuthorizationService(userRepository, null);
		UserId = authService.Register(new UserDto { Password = "examplePassword123!", Login = "exampleLogin123", Email = "test@example.com" });
		return UserId;
	}

	private List<IProduct> products;
    private List<Guid> categoriesGuids;
    private Guid shopId;
    private Guid shopId2;
}
