using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class SearchProductsQueryTests : CatalogueTestsFixture
{
	private InMemoryProductsRepository productsRepository;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService userProductsManagementService;
	private GetUserProductsQueryHandler getUserProductsQueryHandler;

	[SetUp]
	public void SetUp()
	{
		productsRepository = new InMemoryProductsRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(new InMemoryUserProductCategoriesRepository(), productsRepository);
		userProductsManagementService = new UserProductsManagementService(productsRepository, userProductCategoriesService);
		getUserProductsQueryHandler = new GetUserProductsQueryHandler(productsRepository, userProductCategoriesService);
		SeedRepositoryWithSampleProducts();
	}

	private void SeedRepositoryWithSampleProducts()
	{
		var products = new List<IProduct>
		{
			SharedProduct.CreateNew("Milk"),
			SharedProduct.CreateNew("Eggs"),
			SharedProduct.CreateNew("Cream"),
			SharedProduct.CreateNew("Sugar"),
			SharedProduct.CreateNew("Banana"),
			SharedProduct.CreateNew("Apple"),
			SharedProduct.CreateNew("Strawberry"),
			UserProduct.Create("Rice noodles", UserId),
			UserProduct.Create("Rice", UserId),
			UserProduct.Create("Tamarind paste", UserId),
			UserProduct.Create("Coconaut milk", UserId),
			UserProduct.Create("Soy milk", UserId),
			UserProduct.Create("Almond milk", UserId),
		};

		products.ForEach(product => productsRepository.Save(product));
	}

	[Test]
	public void WhenFilterStringIsNotProvided_ReturnUserProductsAndSharedProducts()
	{
		var query = new GetUserProductsQuery(UserId, pageSize: 20);

		var result = getUserProductsQueryHandler.Handle(query);

		Assert.AreEqual(13, result.Count);
	}

	[Test]
	public void WhenFilterStringIsProvided_ReturnProductsThatMeetsCriteria()
	{
		var query = new GetUserProductsQuery(UserId, "milk");

		var result = getUserProductsQueryHandler.Handle(query);

		Assert.AreEqual(4, result.Count);
	}

	[Test]
	public void WhenMaxSizeIsProvided_AssertReturnedProductsCountIsLimited()
	{
		var query = new GetUserProductsQuery(UserId, pageSize: 10);

		var result = getUserProductsQueryHandler.Handle(query);

		Assert.AreEqual(10, result.Count);
	}

	[Test]
	public void WhenPageIsSpecified_AndUserRequestedProductsFromPAgeThatDoNotExist_ReturnEmptyResult()
	{
		var query = new GetUserProductsQuery(UserId, pageSize: 10, page: 5);

		var result = getUserProductsQueryHandler.Handle(query);

		Assert.IsEmpty(result);
	}

	[Test]
	public void WhenPageIsSpecified_AssertElementsFromPageAreReturned()
	{
		var query = new GetUserProductsQuery(UserId, pageSize: 10, page: 2);

		var result = getUserProductsQueryHandler.Handle(query);

		Assert.AreEqual(3, result.Count);
	}


	[Test]
	public void WhenProductHasSpecifiedCategoryAsSubcategory_ReturnProductWithCategoryName()
	{
		var productCategoryParent = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productCategoryChild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(parentId: productCategoryParent, name: "dupa123"));
		var productDto = AUserProduct(categoryId: productCategoryChild);
		var created = userProductsManagementService.DefineNewUserProduct(productDto, UserId);
		var query = new GetUserProductsQuery(UserId, pageSize: 100, page: 1);

		var result = getUserProductsQueryHandler.Handle(query);
		var product = result.First(p => p.Guid == created.Guid);

		Assert.NotNull(product);
		Assert.NotNull(product.Name);
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("dupa123", product.CategoryName);
		Assert.NotNull(product.Guid);
	}

	private UserProductDto AUserProduct(string name = "test", Guid? categoryId = null)
	{
		return new UserProductDto
		{
			Name = name,
			CategoryId = categoryId,
		};
	}
}
