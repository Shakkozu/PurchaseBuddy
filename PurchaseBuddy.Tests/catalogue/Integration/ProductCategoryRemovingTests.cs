using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductCategoryRemovingTests : CatalogueTestsFixture
{
	private InMemoryProductsRepository userProductsRepo;
	private IUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;

	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new ProductCategoriesRepository(TestConfigurationHelper.GetConnectionString());
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);

		UserId = AUserCreated();
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
}
