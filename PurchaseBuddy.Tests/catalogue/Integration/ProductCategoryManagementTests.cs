using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProductCategories;
using System.Transactions;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductCategoryManagementTests : CatalogueTestsFixture
{
	private TransactionScope _transactionScope;

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

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var servicesCollection = new ServiceCollection();
		MigrationsRunner.ClearDatabase(servicesCollection, TestConfigurationHelper.GetConnectionString());
		MigrationsRunner.RunMigrations(servicesCollection, TestConfigurationHelper.GetConnectionString());

		userProductsRepo = new ProductsRepository(TestConfigurationHelper.GetConnectionString());
		userCategoriesRepo = new ProductCategoriesRepository(TestConfigurationHelper.GetConnectionString());
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);

		UserId = AUserCreated();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		base.TearDown();
	}

	[Test]
	public void UserProductCategoriesWithGranchildrenAreReturnedCorrectly()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child1 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root));
		var child2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child2", root));
		userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandchild11", child1));
		userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandchild12", child1));
		var handler = new GetUserProductCategoriesQueryHandler(userCategoriesRepo);

		var rootCategory = userProductCategoriesService.GetUserProductCategories(UserId).Categories.First();

		Assert.NotNull(rootCategory);
		Assert.AreEqual(2, rootCategory.Children.Count);
		Assert.AreEqual(2, rootCategory.Children.First(pc => pc.Name == "child1").Children.Count);
		Assert.AreEqual(0, rootCategory.Children.First(pc => pc.Name == "child2").Children.Count);
	}
	
	[Test]
	public void OnlyRequestingUserProductCategoriesAreReturned()
	{
		var user1 = AUserCreated("testUser1");
		var user2 = AUserCreated("testUser2");
		var user1Category = userProductCategoriesService.AddNewProductCategory(user1, AUserProductCategoryCreateRequest());
		userProductCategoriesService.AddNewProductCategory(user2, AUserProductCategoryCreateRequest());
		new GetUserProductCategoriesQueryHandler(userCategoriesRepo);

		var userCategories = userProductCategoriesService.GetUserProductCategories(user1).Categories;

		Assert.AreEqual(1, userCategories.Count());
		Assert.AreEqual(user1Category, userCategories.First().Guid);
	}

	[Test]
	public void UserCanAddNewProductCategory()
	{
		var createRequest = AUserProductCategoryCreateRequest();

		var createdId = userProductCategoriesService.AddNewProductCategory(UserId, createRequest);

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.True(productCategories.Any(category => category.Guid == createdId));
	}

	[Test]
	public void UserCanAddNewProductCategoryWithParentCategory()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root));
		userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandchild", child));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(1, productCategories.First().Children.Count);
		Assert.AreEqual(1, productCategories.First().Children.First().Children.Count);
	}
	
	[Test]
	public void UserCanAddNewProductCategory_WhenNewCategoryIsOnLevel3OfIndent()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root));
		var grandchild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandchild", child));

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		var lastAddedChild = productCategories.First().Children.First().Children.First();

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(1, productCategories.First().Children.Count);
		Assert.NotNull(lastAddedChild);
	}
	
	[Test]
	public void WhenUserReassignsSubcategory_AssertChildSubSubcategoriesAreAlsoReassigned()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root1"));
		var root2= userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child", root));
		var grandChild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandChild", child));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.IsEmpty(productCategories.First(pc => pc.Guid == root).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children.First().Children);
	}
	
	[Test]
	public void UserCanReassignCategory_WhenCategoryAlreadyHasParent()
	{
		var root1 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root1"));
		var root2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root1));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(2, productCategories.Count);
		Assert.IsEmpty(productCategories.First(pc => pc.Guid == root1).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
	}
	
	[Test]
	public void UserCanReassignCategory_WhenNewParentCategoryIsSharedCategory()
	{
		var root = ASharedCategory("dairy");
		userCategoriesRepo.Save(root);
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child"));

		userProductCategoriesService.ReassignCategory(UserId, child, root.Guid);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.IsNotEmpty(productCategories.First().Children);
	}

	[Test]
	public void UserCanReassignCategory_WhenCategoryDoNotHaveParent()
	{
		var root2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1"));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
	}

	[Test]
	public void GetUserCategories_AssertThereAreNoDuplicates()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("test", root));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.NotNull(productCategories.First().Children);
	}
	
	[Test]
	public void GetUserCategories_WhenThereAreBothSharedAndUserCategories_AssertThereAreNoDuplicates()
	{
		var root = ASharedCategory("dairy");
		var child = ASharedCategory("milk", root);
		userCategoriesRepo.Save(root);
		userCategoriesRepo.Save(child);

		var childUser = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("test", root.Guid));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(2,productCategories.First(c => c.Guid == root.Guid).Children.Count);
		Assert.NotNull(productCategories.First().Children);
	}

	[Test]
	public void CreateProductAndAssignToCategory()
	{
		var productCategory = ProductCategoryCreated("dairy");

		var productDto = new UserProductDto
		{
			CategoryId = productCategory.Guid,
			Name = "Cheese"
		};

		productService.DefineNewUserProduct(productDto, UserId);

		var categoryFromDb = userProductCategoriesService.GetCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
	}

	[Test]
	public void ShouldReturnCategoriesInFlatStructureCorrectlyWithoutDuplicates()
	{
		var root1 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root1"));
		var child11 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest( "child1", root1));
		var child12 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest( "grandchild", root1));
		var grandchild111 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandchild111", child11));
		var root2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child21 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child21", root2));
		var createdCategoriesGuids = new HashSet<Guid> { root1, child11, child21, grandchild111, root2, child21 };

		var categories = userProductCategoriesService.GetCategoriesAsFlatList(UserId).ToList();

		Assert.AreEqual(6, categories.Count());
		Assert.True(createdCategoriesGuids.All(createdCategoryGuid => categories.Any(category => category.Guid == createdCategoryGuid)));
	}

	private IProductCategory ProductCategoryCreated(string name)
	{
		var category = UserProductCategory.CreateNew(name, UserId);
		return userCategoriesRepo.Save(category);
	}


	private IProductsRepository userProductsRepo;
	private IUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;
}
