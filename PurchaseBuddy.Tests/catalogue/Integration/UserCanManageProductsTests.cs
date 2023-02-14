using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class UserCanManageProductsTests
{
	[SetUp]
	public void SetUp()
	{
		userProductsManagementService = new UserProductsManagementService(new InMemoryUserProductsRepository());
	}

	[Test]
	public void CanAddProduct()
	{
		var product = UserProduct.Create("eggs", userId);

		userProductsManagementService.DefineNewUserProduct(product);
		var products = userProductsManagementService.GetUserProducts(userId);

		Assert.Contains(product, products);
	}

	private readonly Guid userId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
	private UserProductsManagementService userProductsManagementService;
}
