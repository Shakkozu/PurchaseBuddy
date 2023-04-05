using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductCategoryRemovingTests : CatalogueTestsFixture
{
	private InMemoryProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;

	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);
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
