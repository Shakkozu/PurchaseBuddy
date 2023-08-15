using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.API;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.crm;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddy.Tests.purchases.Integration;

internal class AllowedUserHasAccessToShoppingListManagementTests : PurchaseBuddyTestsFixture
{
	private IUserProductsManagementService productsManagementService;
	private IUserShopService shopService;
	private IUserProductCategoriesManagementService categoriesManagementService;
	private IShoppingListReadService shoppingListReadService;
	private IShoppingListWriteService shoppingListWriteService;
	private List<IProduct> products;
	private Guid shopId;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var services = new ServiceCollection();
		PurchaseBuddyFixture.RegisterDependencies(services, null);
		var usersProvider = new UsersProvider(new UserRepository(TestConfigurationHelper.GetConnectionString()));
		services.AddSingleton<IUsersProvider>(usersProvider);
		var serviceProvider = services.BuildServiceProvider();

		productsManagementService = serviceProvider.GetRequiredService<IUserProductsManagementService>();
		shopService = serviceProvider.GetRequiredService<IUserShopService>();
		categoriesManagementService = serviceProvider.GetRequiredService<IUserProductCategoriesManagementService>();
		shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();
		shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();

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
	public void AllowedUserShouldBeAbleToReadSharedShoppingLists()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		var lists = shoppingListReadService.GetAllShoppingLists(otherUser);

		Assert.IsNotEmpty(lists);
	}

	[Test]
	public void ShouldNotReturnListToForeignUser_WhenFetchingList()
	{
		AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();

		var lists = shoppingListReadService.GetAllShoppingLists(otherUser);

		Assert.IsEmpty(lists);
	}

	[Test]
	public void AllowedUserShouldBeAbleToReadSharedShoppingList()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);

		Assert.IsNotNull(list);
	}

	[Test]
	public void AllowedUserShouldBeAbleToAddNewListItemToList()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ProductCategoryName = "Fruits" });

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		var addedProduct = list.ShoppingListItems.Find(item => item.ProductDto.Name == "Banana");
		Assert.NotNull(addedProduct);
		Assert.AreEqual("Fruits", addedProduct.ProductDto.CategoryName);
	}

	[Test]
	public void AllowedUserShouldBeAbleToAddNewListItemsToList()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ProductCategoryName = "Fruits" });
		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductName = "Chocolate", ProductCategoryName = "Sweets" });

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		Assert.AreEqual(3, list.ShoppingListItems.Count);
	}

	[Test]
	public void AllowedUserShouldBeAbleToAddNewListItemToList_WhenProductIdIsProvided_ShouldSaveImportedProduct()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();
		var otherUserProduct = productsManagementService.DefineNewUserProduct(UserProduct.Create("Banana", otherUser));
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		shoppingListWriteService.AddNewListItem(otherUser, shoppingListId, new AddNewListItemRequest { ProductGuid = otherUserProduct.Guid });

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		var addedProduct = list.ShoppingListItems.Find(item => item.ProductDto.Name == "Banana");
		Assert.NotNull(addedProduct);
	}

	[Test]
	public void NotAllowedUserShouldNotBeAbleToAddNewListItemToList()
	{
		var shoppingListId = AShoppingListWithSingleItemCreated();
		var otherUser = ANewUserCreated();

		Assert.Throws<ArgumentNullException>(() =>
			shoppingListWriteService.AddNewListItem(otherUser,
			shoppingListId,
			new AddNewListItemRequest { ProductName = "Banana", ProductCategoryName = "Fruits" }));
	}

	[Test]
	public void AllowedUserShouldBeAbleToRemoveListItemFromList()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		shoppingListWriteService.RemoveItemFromList(otherUser, shoppingListId, secondItemGuid);

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		Assert.AreEqual(1, list.ShoppingListItems.Count);
	}

	[Test]
	public void NotAllowedUserShouldNotBeAbleToRemoveListItemFromList()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });

		Assert.Throws<ArgumentException>(() => shoppingListWriteService.RemoveItemFromList(Guid.NewGuid(), shoppingListId, secondItemGuid));
	}

	[Test]
	public void AllowedUserShouldBeAbleToMarkItemAsPurchased()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);

		shoppingListWriteService.MarkListItemAsPurchased(otherUser, shoppingListId, secondItemGuid);

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		Assert.AreEqual(true, list.ShoppingListItems.Find(item => Guid.Parse(item.Guid) == secondItemGuid)?.Purchased);
	}

	[Test]
	public void NotAllowedUserShouldNotBeAbleToMarkItemAsPurchased()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });

		Assert.Throws<ArgumentException>(() => shoppingListWriteService.MarkListItemAsPurchased(Guid.NewGuid(), shoppingListId, secondItemGuid));
	}
	
	[Test]
	public void AllowedUserShouldBeAbleToMarkItemAsNotPurchased()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });
		var otherUser = ANewUserCreated();
		shoppingListWriteService.GrantAccessToModifyingList(shoppingListId, otherUser);
		shoppingListWriteService.MarkListItemAsPurchased(otherUser, shoppingListId, secondItemGuid);

		shoppingListWriteService.MarkListItemAsNotPurchased(otherUser, shoppingListId, secondItemGuid);

		var list = shoppingListReadService.GetShoppingList(otherUser, shoppingListId);
		Assert.AreEqual(false, list.ShoppingListItems.Find(item => Guid.Parse(item.Guid) == secondItemGuid)?.Purchased);
	}

	[Test]
	public void NotAllowedUserShouldNotBeAbleToMarkItemAsNotPurchased()
	{
		var secondItemGuid = Guid.NewGuid();
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Banana", ListItemGuid = secondItemGuid });

		Assert.Throws<ArgumentException>(() => shoppingListWriteService.MarkListItemAsNotPurchased(Guid.NewGuid(), shoppingListId, secondItemGuid));
	}

	private Guid AShoppingListWithSingleItemCreated()
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
