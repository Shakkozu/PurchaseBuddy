using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddy.Tests.catalogue;


internal class UserCanManageProductsTests
{
	[SetUp]
	public void SetUp()
	{
		userProductCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductsManagementService = new UserProductsManagementService(new InMemoryUserProductsRepository(), userProductCategoriesRepo);
	}

	[Test]
	public void CanAddProduct()
	{
		var product = UserProduct.Create("eggs", userId);

		userProductsManagementService.DefineNewUserProduct(product);
		List<UserProduct> products = userProductsManagementService.GetUserProducts(userId);

		Assert.Contains(product, products);
	}

	private Guid userId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
	private InMemoryUserProductCategoriesRepository userProductCategoriesRepo;
	private UserProductsManagementService userProductsManagementService;
}
