using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.App.Queries;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class SearchProductsQueryTests : CatalogueTestsFixture
{
	private InMemoryProductsRepository productsRepository;
	private UserProductsManagementService userProductsManagementService;
	private GetUserProductsQueryHandler getUserProductsQueryHandler;

	[SetUp]
	public void SetUp()
	{
		productsRepository = new InMemoryProductsRepository();
		userProductsManagementService = new UserProductsManagementService(productsRepository, null);
		getUserProductsQueryHandler = new GetUserProductsQueryHandler(productsRepository);
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

	
}
