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

namespace PurchaseBuddy.Tests.purchases.Integration;

internal class ShoppingListServiceTests : PurchaseBuddyTestsFixture
{
	private ServiceProvider serviceProvider;
	private IUserProductsManagementService productsManagementService;
	private IUserShopService shopService;
	private IUserProductCategoriesManagementService categoriesManagementService;
	private IShoppingListWriteService shoppingListWriteService;
	private IShoppingListReadService shoppingListReadService;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var services = new ServiceCollection();
		PurchaseBuddyFixture.RegisterDependencies(services, TestConfigurationHelper.GetConnectionString());
		serviceProvider = services.BuildServiceProvider();

		Extensions.RecordElapsedTime("setup database", () =>
		{
			MigrationsRunner.ClearDatabase(services, TestConfigurationHelper.GetConnectionString());
			MigrationsRunner.RunMigrations(services, TestConfigurationHelper.GetConnectionString());
		});
	}

	[SetUp]
	public void Setup()
	{
		productsManagementService = serviceProvider.GetRequiredService<IUserProductsManagementService>();
		shopService = serviceProvider.GetRequiredService<IUserShopService>();
		categoriesManagementService = serviceProvider.GetRequiredService<IUserProductCategoriesManagementService>();
		shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();
		shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();

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
	public void ShouldAddImportedProductToList()
	{
		var importedShoppingListItems = new List<ShoppingListItem>
		{
			ImportedShoppingListItem.CreateNew("Milk", "Dairy")
		};
		var createdListId = shoppingListWriteService.CreateNewList(UserId, importedShoppingListItems);
		var addNewItemRequest = new AddNewListItemRequest
		{
			ProductName = "testProduct",
			ProductCategoryName = "testCategory"
		};

		shoppingListWriteService.AddNewListItem(UserId, createdListId, addNewItemRequest);

		var createdList = shoppingListReadService.GetShoppingList(UserId, createdListId);
		Assert.AreEqual(createdList.ShoppingListItems.Last().ProductDto.Name, "testProduct");
	}
	
	[Test]
	public void ShouldAddUsedDefinedProductToList()
	{
		var importedShoppingListItems = new List<ShoppingListItem>
		{
			ImportedShoppingListItem.CreateNew("Milk", "Dairy")
		};
		var createdListId = shoppingListWriteService.CreateNewList(UserId, importedShoppingListItems);
		var addNewItemRequest = new AddNewListItemRequest
		{
			ProductGuid = products.First().Guid
		};

		shoppingListWriteService.AddNewListItem(UserId, createdListId, addNewItemRequest);

		var createdList = shoppingListReadService.GetShoppingList(UserId, createdListId);
		Assert.AreEqual(createdList.ShoppingListItems.Last().ProductDto.Name, products.First().Name);
	}

	[Test]
	public void ShouldSaveImportedShoppingListItemCorrectly()
	{
		var importedShoppingListItems = new List<ShoppingListItem>
		{
			ImportedShoppingListItem.CreateNew("Milk", "Dairy")
		};

		var createdListId = shoppingListWriteService.CreateNewList(UserId, importedShoppingListItems);

		var createdList = shoppingListReadService.GetShoppingList(UserId, createdListId);
		Assert.AreEqual(createdList.ShoppingListItems.First().ProductDto.Name, ((ImportedShoppingListItem)importedShoppingListItems.First()).ProductName);
	}

	[Test]
	public void ShouldPurchaseImportedListItemProperly()
	{
		var listItem = ImportedShoppingListItem.CreateNew("Milk", "Dairy");
		var importedShoppingListItems = new List<ShoppingListItem> { listItem };
		var createdListId = shoppingListWriteService.CreateNewList(UserId, importedShoppingListItems);

		shoppingListWriteService.MarkListItemAsPurchased(UserId, createdListId, listItem.Guid);

		var createdList = shoppingListReadService.GetShoppingList(UserId, createdListId);
		Assert.True(createdList.ShoppingListItems.First().Purchased);
	}

	[Test]
	public void WhenListIsAssignedToDeactivatedShop_ListIsReturnedWithoutShopInfo()
	{
		var shop = AShopWithCategories();
		var list = shoppingListWriteService.CreateNewList(UserId, AListItemsWithSingleItem(), shop);
		shopService.DeleteUserShop(UserId, shop);

		var userList = shoppingListReadService.GetShoppingList(UserId, list);

		Assert.AreEqual(null, userList.AssignedShop);
	}

	[Test]
	public void ShouldOrderShoppingListItemsByAssignedShopCategoriesConfig()
	{
		Extensions.RecordElapsedTime("Test Execution", () =>
		{
			Guid category1 = categoriesGuids[2], category2 = categoriesGuids[0], category3 = categoriesGuids[1];
			var productCat1 = AProductWithCategory(category1);
			var productCat2 = AProductWithCategory(category2);
			var productCat3 = AProductWithCategory(category3);
			var shop = AShopWithCategories(new[] { category1, category2, category3 });
			var listItems = AListItems(new[] { productCat1, productCat2, productCat3 });
			var list = shoppingListWriteService.CreateNewList(UserId, listItems, shop);

			var userList = shoppingListReadService.GetShoppingList(UserId, list);

			Assert.AreEqual(productCat1, userList.ShoppingListItems[0].ProductDto.Guid);
			Assert.AreEqual(productCat2, userList.ShoppingListItems[1].ProductDto.Guid);
			Assert.AreEqual(productCat3, userList.ShoppingListItems[2].ProductDto.Guid);
		});
	}

	[Test]
	public void ShouldSaveShoppingListsFieldsCorrectly()
	{
		var listItems = AListItems();
		var list = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, list);

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
		var list = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		var list2 = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkListItemAsPurchased(UserId, list, listItems.First().Guid);
		var userLists = shoppingListReadService.GetNotClosedShoppingLists(UserId);

		Assert.AreEqual(1, userLists.Count);
		Assert.AreEqual(list2, userLists.First().Guid);
	}

	[Test]
	public void ShouldNotCreateEmptyList()
	{
		var listItems = new List<ShoppingListItem> { };

		Assert.Throws<InvalidOperationException>(() => shoppingListWriteService.CreateNewList(UserId, listItems, shopId));
	}

	[Test]
	public void ShouldMarkDifferentListItemTypesAsPurchased()
	{
		var importedListItem = ImportedShoppingListItem.CreateNew("Milk", "Dairy");
		var normalListItem = ShoppingListItem.CreateNew(products.First().Guid);
		var listItems = new List<ShoppingListItem> { importedListItem, normalListItem };
		var createdListId = shoppingListWriteService.CreateNewList(UserId, listItems);
		shoppingListWriteService.MarkListItemAsPurchased(UserId, createdListId, importedListItem.Guid);
		shoppingListWriteService.MarkListItemAsPurchased(UserId, createdListId, normalListItem.Guid);

		var createdList = shoppingListReadService.GetShoppingList(UserId, createdListId);

		Assert.True(createdList.Completed);
	}

	[Test]
	public void ShouldMarkListItemAsNotPurchased()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);
		shoppingListWriteService.MarkListItemAsPurchased(UserId, list, productGuid);

		shoppingListWriteService.MarkListItemAsNotPurchased(UserId, list, productGuid);

		var listItem = shoppingListReadService.GetShoppingList(UserId, list).ShoppingListItems.First(li => li.ProductDto.Guid == productGuid);
		Assert.False(listItem.Purchased);
	}

	[Test]
	public void ShouldContainInformationAboutAssignedShop()
	{
		var productGuid = products.First().Guid;

		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		var listFromDb = shoppingListReadService.GetShoppingList(UserId, list);
		Assert.AreEqual("test1", listFromDb.AssignedShop.Name);
	}

	[Test]
	public void ShouldContainInformationsAboutAssignedShop()
	{
		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, list);

		Assert.NotNull(userList.AssignedShop.Name);
	}


	[Test]
	public void ShouldChangeProductQuantity()
	{
		var listItems = AListItems();
		var listItemGuid = listItems.First().Guid;
		var newQuantity = 10;
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.ChangeQuantityOfProductOnList(UserId, listId, listItemGuid, 10);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.AreEqual(newQuantity, userList.ShoppingListItems.First(p => p.Guid == listItemGuid.ToString()).Quantity);
	}


	[Test]
	public void ShouldMarkProductAsUnavailable()
	{
		var listItems = AListItems();
		var listItemGuid = listItems.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkListItemtAsUnavailable(UserId, listId, listItemGuid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.ShoppingListItems.First(p => p.Guid == listItemGuid.ToString()).Unavailable);
	}

	[Test]
	public void ShouldCreateNewListWithNotPurchasedItems()
	{
		var listItems = AListItems();
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkListItemtAsUnavailable(UserId, listId, listItems.First().Guid);
		var newListId = shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId2);

		var userList = shoppingListReadService.GetShoppingList(UserId, newListId);
		Assert.AreEqual(shopId2, userList.AssignedShop.Guid);
		Assert.AreEqual(userList.ShoppingListItems.First().ProductDto.Guid, listItems.First().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.First().Quantity, listItems.First().Quantity);
		Assert.AreEqual(userList.ShoppingListItems.Last().ProductDto.Guid, listItems.Last().ProductId);
		Assert.AreEqual(userList.ShoppingListItems.Last().Quantity, listItems.Last().Quantity);
	}

	[Test]
	public void ShouldNotCreateNewListWithNotPurchasedItems_WhenAllItemsArePurchased()
	{
		var listItems = AListItemsWithSingleItem();
		var listItemGuid = listItems.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);
		shoppingListWriteService.MarkListItemAsPurchased(UserId, listId, listItemGuid);

		Assert.Throws<InvalidOperationException>(() => shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId2));
	}

	[Test]
	public void ShouldCreateNewListWithNotPurchasedItemsAndCompleteCurrentList()
	{
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
	}


	[Test]
	public void ShouldCreateNewListWithNotPurchasedItemsAndRemoveNotCompletedItems()
	{
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.IsEmpty(userList.ShoppingListItems);
	}

	[Test]
	public void ShouldMarkListAsCompletedWhenLastProductIsPurchased()
	{
		var listItems = AListItemsWithSingleItem();
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkListItemAsPurchased(UserId, listId, listItems.First().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

	[Test]
	public void ShouldCompleteListWhenLastNotPurchasedProductIsRemoved()
	{
		var listItems = AListItems();
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkListItemAsPurchased(UserId, listId, listItems.First().Guid);
		shoppingListWriteService.RemoveItemFromList(UserId, listId, listItems.Last().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

	[Test]
	public void ShouldRemoveProductFromList()
	{
		var listItems = AListItems();
		var list = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.RemoveItemFromList(UserId, list, listItems.First().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, list);
		Assert.AreEqual(1, userList.ShoppingListItems.Count);
	}

	private List<ShoppingListItem> AListItems(IEnumerable<Guid> _products = null)
	{
		if (_products != null)
			return _products.Select(product => ShoppingListItem.CreateNew(product)).ToList();

		var listItems = new List<ShoppingListItem>
		{
			ShoppingListItem.CreateNew(products.First().Guid, 1),
			ShoppingListItem.CreateNew(products.Last().Guid, 2),
		};

		return listItems;
	}

	private List<ShoppingListItem> AListItemsWithSingleItem()
	{
		return new List<ShoppingListItem>
		{
			ShoppingListItem.CreateNew(products.First().Guid, 1),
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
	private Guid AShopWithCategories()
	{
		return shopService.AddNew(UserId, UserShopDescription.CreateNew("test"), new List<Guid>());
	}

	private List<IProduct> products;
	private List<Guid> categoriesGuids;
	private Guid shopId;
	private Guid shopId2;
}
