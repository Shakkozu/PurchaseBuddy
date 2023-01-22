using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddy.Tests.catalogue;

internal class ProductCategoryManagementTests : CatalogueTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryUserProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductsManagementService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
	}

	[Test]
	public void CanAddNewProductCategory()
	{
		var productCategory = AUserProductCategory();

		userProductsManagementService.AddNewProductCategory(productCategory);
		List<UserProductCategory> productCategories = userProductsManagementService.GetUserProductCategories(UserId);

		Assert.Contains(productCategory, productCategories);
	}

	[Test]
	public void WhenProductAddedToCategory_AssertProductIsShownInCategory()
	{
		var productCategory = ProductCategoryCreated("dairy");
		var product = ProductCreated("cheese");

		userProductsManagementService.AssignUserProductToCategory(UserId, product.Guid, productCategory.Guid);

		var categoryFromDb = userProductsManagementService.GetUserProductCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
		Assert.True(categoryFromDb.ContainsProductWithGuid(product.Guid));
	}

	private UserProductCategory ProductCategoryCreated(string name)
	{
		var category = UserProductCategory.CreateNew(name, UserId);
		return userCategoriesRepo.Save(category);
	}

	private UserProduct ProductCreated(string name)
	{
		var product = UserProduct.Create(name, UserId);
		return userProductsRepo.Save(product);
	}


	private InMemoryUserProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductsManagementService;
}
