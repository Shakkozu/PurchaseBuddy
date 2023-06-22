using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.API;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddy.Tests.purchases.Integration;
internal class ShoppingListServiceTests : PurchaseBuddyTestsFixture
{
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
		var serviceProvider = services.BuildServiceProvider();

		productsManagementService = serviceProvider.GetRequiredService<IUserProductsManagementService>();
		shopService = serviceProvider.GetRequiredService<IUserShopService>();
		categoriesManagementService = serviceProvider.GetRequiredService<IUserProductCategoriesManagementService>();
		shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();
		shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();
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
	public void WhenListIsAssignedToDeactivatedShop_ListIsReturnedWithoutShopInfo()
	{
		Guid shop = Guid.NewGuid();
		shop = AShopWithCategories();
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

		shoppingListWriteService.MarkProductAsPurchased(UserId, list, listItems.First().ProductId);
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
	public void ShouldMarkListItemAsPurchased()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.MarkProductAsPurchased(UserId, list, productGuid);

		var listItem = shoppingListReadService.GetShoppingList(UserId, list).ShoppingListItems.First(li => li.ProductDto.Guid == productGuid);
		Assert.True(listItem.Purchased);
	}

	[Test]
	public void ShouldMarkListItemAsNotPurchased()
	{
		var productGuid = products.First().Guid;
		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.MarkProductAsPurchased(UserId, list, productGuid);
		shoppingListWriteService.MarkProductAsNotPurchased(UserId, list, productGuid);

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
		var productGuid = products.First().Guid;
		var newQuantity = 10;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.ChangeQuantityOfProductOnList(UserId, listId, productGuid, 10);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.AreEqual(newQuantity, userList.ShoppingListItems.First(p => p.ProductDto.Guid == productGuid).Quantity);
	}


	[Test]
	public void ShouldMarkProductAsUnavailable()
	{
		var productGuid = products.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.MarkProductAsUnavailable(UserId, listId, productGuid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.ShoppingListItems.First(p => p.ProductDto.Guid == productGuid).Unavailable);
	}

	[Test]
	public void ShouldUpdateProductsList()
	{
		var productGuid = products.First().Guid;
		var newProductGuid = products.Last().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(new List<Guid> { productGuid }));

		shoppingListWriteService.UpdateProductsOnList(UserId, listId, new List<Guid> { newProductGuid});

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.ShoppingListItems.All(p => p.ProductDto.Guid == newProductGuid));
	}

	[Test]
	public void ShouldCreateNewListWithNotPurchasedItems()
	{
		var productGuid = products.First().Guid;
		var listItems = AListItems();
		var listId = shoppingListWriteService.CreateNewList(UserId, listItems, shopId);

		shoppingListWriteService.MarkProductAsUnavailable(UserId, listId, productGuid);
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
		var productGuid = products.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItemsWithSingleItem(), shopId);

		shoppingListWriteService.MarkProductAsPurchased(UserId, listId, productGuid);

		Assert.Throws<InvalidOperationException>(() => shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId2));
	}

	[Test]
	public void ShouldCreateNewListWithNotPurchasedItemsAndCompleteCurrentList()
	{
		var productGuid = products.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
	}


	[Test]
	public void ShouldCreateNewListWithNotPurchasedItemsAndRemoveNotCompletedItems()
	{
		var productGuid = products.First().Guid;
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.CreateNewListWithNotBoughtItems(UserId, listId, shopId);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.IsEmpty(userList.ShoppingListItems);
	}

	[Test]
	public void ShouldMarkListAsCompletedWhenLastProductIsPurchased()
	{
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItemsWithSingleItem(), shopId);

		shoppingListWriteService.MarkProductAsPurchased(UserId, listId, products.First().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

	[Test]
	public void ShouldCompleteListWhenLastNotPurchasedProductIsRemoved()
	{
		var listId = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.MarkProductAsPurchased(UserId, listId, products.First().Guid);
		shoppingListWriteService.RemoveProductFromList(UserId, listId, products.Last().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, listId);
		Assert.True(userList.Completed);
		Assert.NotNull(userList.CompletedAt);
	}

	[Test]
	public void ShouldRemoveProductFromList()
	{
		var list = shoppingListWriteService.CreateNewList(UserId, AListItems(), shopId);

		shoppingListWriteService.RemoveProductFromList(UserId, list, products.First().Guid);

		var userList = shoppingListReadService.GetShoppingList(UserId, list);
		Assert.AreEqual(1, userList.ShoppingListItems.Count);
	}

	private List<ShoppingListItem> AListItems(IEnumerable<Guid> _products = null)
	{
		if (_products != null)
			return _products.Select(product => ShoppingListItem.CreateNew(product)).ToList();

		return new List<ShoppingListItem>
	{
		ShoppingListItem.CreateNew(products.First().Guid, 1),
		ShoppingListItem.CreateNew(products.Last().Guid, 2),
	};
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
