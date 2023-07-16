using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

namespace PurchaseBuddy.Tests.purchases.Integration;

internal class ShoppingListSharingTests : PurchaseBuddyTestsFixture
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

	[Test]
	public void ShouldReturnSharedListOnListsCollection()
	{
		var shoppingListId = AShoppingListCreated();
		var otherUser = ANewUserCreated();
		shoppingListWriteService.ShareList(UserId, shoppingListId, otherUser);

		var lists = shoppingListReadService.GetAllShoppingLists(otherUser);

		Assert.IsNotEmpty(lists);
	}

	[Test]
	public void ShouldReturnSharedList()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var shoppingListId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var otherUser = ANewUserCreated();
		shoppingListWriteService.ShareList(UserId, shoppingListId, otherUser);

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);

		Assert.IsNotNull(list);
	}

	[Test]
	public void AllowedUserShouldBeAbleToAddNewListItemToList()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var shoppingListId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var otherUser = ANewUserCreated();
		shoppingListWriteService.ShareList(UserId, shoppingListId, otherUser);

		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ProductCategoryName = "Fruits" });

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		var addedProduct = list.ShoppingListItems.Find(item => item.ProductDto.Name == "Banana");
		Assert.NotNull(addedProduct);
		Assert.AreEqual("Fruits", addedProduct.ProductDto.CategoryName);
	}

	[Test]
	public void AllowedUserShouldBeAbleToAddNewListItemToList_WhenProductIdIsProvided_ShouldSaveImportedProduct()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var shoppingListId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var otherUser = ANewUserCreated();
		var otherUserProduct = productsManagementService.DefineNewUserProduct(UserProduct.Create("Banana", otherUser));
		shoppingListWriteService.ShareList(UserId, shoppingListId, otherUser);

		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductGuid = otherUserProduct.Guid });

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		var addedProduct = list.ShoppingListItems.Find(item => item.ProductDto.Name == "Banana");
		Assert.NotNull(addedProduct);
	}

	[Test]
	public void NotAllowedUserShouldNotBeAbleToAddNewListItemToList()
	{
		var product = productsManagementService.DefineNewUserProduct(UserProduct.Create("Milk", UserId));
		var listItems = new List<ShoppingListItem> { ShoppingListItem.CreateNew(product.Guid) };
		var shoppingListId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var otherUser = ANewUserCreated();

		Assert.Throws<ArgumentNullException>(() => 
			shoppingListWriteService.AddNewListItem(otherUser,
			shoppingListId, 
			new AddNewListItemRequest { ProductName = "Banana", ProductCategoryName = "Fruits" }));
	}

	private Guid AShoppingListCreated()
	{
		var shoppingListItem = ShoppingListItem.CreateNew(products.First().Guid);
		return shoppingListWriteService.CreateNewList(UserId, new List<ShoppingListItem> { shoppingListItem });
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
