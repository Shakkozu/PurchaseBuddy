using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class UserCanManageCategoriesTests : CatalogueTestsFixture
{

    private IProductsRepository userProductsRepo;
    private InMemoryUserProductCategoriesRepository userCategoriesRepo;
    private UserProductCategoriesManagementService userProductsCategoryService;

    [SetUp]
    public void SetUp()
    {
        userProductsRepo = new InMemoryProductsRepository();
        userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
        userProductsCategoryService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
    }

    protected IProductCategory AProductCategory(string? name = null)
    {
        return UserProductCategory.CreateNew(name ?? "dairy", UserId);
    }
}
