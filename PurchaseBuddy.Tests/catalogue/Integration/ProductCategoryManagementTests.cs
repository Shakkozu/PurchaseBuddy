using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.infra;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductCategoryManagementTests : CatalogueTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);
	}

	[Test]
	public void TestSeed()
	{
		var seed = new SeedSharedProductsDatabase(productService, userProductCategoriesService, userProductsRepo, userCategoriesRepo);
		seed.Seed();

		var userProductsCategories = userCategoriesRepo.FindAll(UserId);
		Assert.IsNotEmpty(userProductsCategories);
	}
	
	[Test]
	public void CanAddNewProductCategory()
	{
		var productCategory = AUserProductCategory();

		userProductCategoriesService.AddNewProductCategory(productCategory);
		var productCategories = userProductCategoriesService.GetUserProductCategories(UserId);

		Assert.Contains(productCategory, productCategories);
	}

	[Test]
	public void WhenProductAddedToCategory_AssertProductIsShownInCategory()
	{
		var productCategory = ProductCategoryCreated("dairy");
		var product = ProductCreated("cheese");

		userProductCategoriesService.AssignUserProductToCategory(UserId, product.Guid, productCategory.Guid);

		var categoryFromDb = userProductCategoriesService.GetUserProductCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
		Assert.True(categoryFromDb.ContainsProductWithGuid(product.Guid));
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

		var categoryFromDb = userProductCategoriesService.GetUserProductCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
		Assert.IsNotEmpty(categoryFromDb.GetProductsInCategory());
	}

	private IProductCategory ProductCategoryCreated(string name)
	{
		var category = UserProductCategory.CreateNew(name, UserId);
		return userCategoriesRepo.Save(category);
	}

	private IProduct ProductCreated(string name)
	{
		var product = UserProduct.Create(name, UserId);
		return userProductsRepo.Save(product);
	}


	private InMemoryProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;
}
