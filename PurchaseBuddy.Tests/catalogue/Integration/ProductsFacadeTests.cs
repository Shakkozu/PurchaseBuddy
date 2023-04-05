using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductsFacadeTests : CatalogueTestsFixture
{
	private ProductsFacade facade;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;

	[SetUp]
	public void SetUp()
	{
		var userProductsRepo = new InMemoryProductsRepository();
		var userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);
		facade = new ProductsFacade(userProductCategoriesService, productService);
	}

	[Test]
	public void RemoveCategoryAndReassignProducts_WhenSourceCategoryDoNotExist_ThrowException() 
	{
		Assert.Throws<ResourceNotFoundException>(() => facade.RemoveCategoryAndReassignProducts(UserId, Guid.NewGuid()));
	}

	[Test]
	public void RemoveCategoryAndReassignProducts_WhenSubstituteCategoryDoNotExist_ThrowException() 
	{
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());

		Assert.Throws<ResourceNotFoundException>(() => facade.RemoveCategoryAndReassignProducts(UserId, category, Guid.NewGuid()));
	}

	[Test]
	public void RemoveCategoryAndReassignProducts_WhenSubstituteCategoryIsNotSpecified_AssertAllProductsWereRemovedFromCategories() 
	{
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var product1 = AProduct("milk", category);
		var product2 = AProduct("dairy", category);

		facade.RemoveCategoryAndReassignProducts(UserId, category);

		var products = productService.GetUserProducts(new PurchaseBuddyLibrary.src.catalogue.App.Queries.GetUserProductsQuery(UserId));
		Assert.AreEqual(2, products.Count);
		Assert.True(products.All(p => p.CategoryId == null));
	}

	[Test]
	public void RemoveCategoryAndReassignProducts_WhenSubstituteCategoryIsProvided_ReassignAllProductsToSubstituteCategory() 
	{
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var substituteCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var product1 = AProduct("milk", category);
		var product2 = AProduct("dairy", category);

		facade.RemoveCategoryAndReassignProducts(UserId, category, substituteCategory);

		var products = productService.GetUserProducts(new PurchaseBuddyLibrary.src.catalogue.App.Queries.GetUserProductsQuery(UserId));
		Assert.AreEqual(2, products.Count);
		Assert.True(products.All(p => p.CategoryId == substituteCategory));
	}

	[Test]
	public void RemoveCategoryAndReassignProducts_AssertCategoryIsRemoved() 
	{
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());

		facade.RemoveCategoryAndReassignProducts(UserId, category);

		var userCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.AreEqual(0, userCategories.Count);
	}

	private IProduct AProduct(string name, Guid categoryID)
	{
		var product = UserProduct.Create(name, UserId, categoryID);
		return productService.DefineNewUserProduct(product);
	}
}
