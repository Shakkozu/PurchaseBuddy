using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductsManagementsTests : CatalogueTestsFixture
{
	private InMemoryProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductsManagementService;

	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductsManagementService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
	}

	[Test]
	public void CreateProductsAndAssignCategories()
	{

	}
}
