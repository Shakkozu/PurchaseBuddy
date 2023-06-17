using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using System.Transactions;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductCategoryRemovingTests : CatalogueTestsFixture
{
	private IProductsRepository userProductsRepo;
	private IUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private TransactionScope _transactionScope;
	private AuthorizationService _authorizationService;


	[SetUp]
	public void SetUp()
	{
		_transactionScope = new TransactionScope();
	}

	[TearDown]
	public override void TearDown()
	{
		_transactionScope.Dispose();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		base.TearDown();
	}

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var connectionString = TestConfigurationHelper.GetConnectionString();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

		userProductsRepo = new ProductsRepository(connectionString);
		userCategoriesRepo = new ProductCategoriesRepository(connectionString);
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		_authorizationService = new AuthorizationService(new UserRepository(connectionString), config);

		var servicesCollection = new ServiceCollection();
		MigrationsRunner.ClearDatabase(servicesCollection, connectionString);
		MigrationsRunner.RunMigrations(servicesCollection, connectionString);
		UserId = AUserCreated();
	}

	[Test]
	public void UserCanRemoveProductCategory_WhenProductCategoryDoesNotHaveChildrenOrParent()
	{
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.IsNotEmpty(productCategories);

		userProductCategoriesService.DeleteCategory(UserId, category);
		productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.IsEmpty(productCategories);
	}

	[Test]
	public void UserCanRemoveProductCategory_WhenProductCategoryHasParent_AssertItsNotReturnedAfterRemoving()
	{
		var rootCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var subCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child", rootCategory));

		userProductCategoriesService.DeleteCategory(UserId, subCategory);

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.AreEqual(1, productCategories.Count);
		Assert.IsEmpty(productCategories.First().Children);
	}


	[Test]
	public void UserCanRemoveProductCategory_WhenProductCategoryHasChildrenAndIsRootCategory_AssertChildrenAreMovedToRootNode()
	{
		var rootCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var subCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child", rootCategory));

		userProductCategoriesService.DeleteCategory(UserId, rootCategory);

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(subCategory, productCategories.First().Guid);
		Assert.IsEmpty(productCategories.First().Children);
	}

	[Test]
	public void UserCanRemoveProductCategory_WhenProductCategoryHasChildrenAndParent_AssertChildrenAreMovedToRemovedNodeParent()
	{
        var rootCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var subCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child", rootCategory));
		var subSubCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandChild", subCategory));

		userProductCategoriesService.DeleteCategory(UserId, subCategory);

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.AreEqual(1, productCategories.Count);
		Assert.IsNotEmpty(productCategories.First().Children);
		Assert.AreEqual(subSubCategory, productCategories.First().Children.First().Guid);
	}
}
