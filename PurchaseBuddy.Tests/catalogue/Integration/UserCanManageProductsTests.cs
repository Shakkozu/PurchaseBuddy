using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class UserCanManageProductsTests : CatalogueTestsFixture
{
    [SetUp]
    public void SetUp()
    {
        userProductsManagementService = new UserProductsManagementService(new InMemoryProductsRepository(), null);
    }

    [Test]
    public void UserCanChangeProductCategoryAssignmentFromSharedCategoryToUserCategory()
    {
        var userProduct = UserProduct.Create("eggs", UserId);
        var userProductCategory = AUserProductCategory("user_dairy");
        var sharedProductCategory = ASharedCategory("dairy");

        userProductCategory.AddProduct(userProduct);
        Assert.True(userProductCategory.ContainsProductWithGuid(userProduct.Guid));

        userProductCategory.RemoveProduct(userProduct);
        sharedProductCategory.AddProduct(userProduct);

        Assert.False(userProductCategory.ContainsProductWithGuid(userProduct.Guid));
        Assert.True(sharedProductCategory.ContainsProductWithGuid(userProduct.Guid));
    }

    [Test]
    public void UserProductCanBeAssignedToSharedProductCategory()
    {
        var userProduct = UserProduct.Create("eggs", UserId);
        var userProductCategory = ASharedCategory("dairy");

        userProductCategory.AddProduct(userProduct);

        Assert.True(userProductCategory.ContainsProductWithGuid(userProduct.Guid));
    }

    [Test]
    public void UserProductCanBeAssignedToUserProductCategory()
    {
        var userProduct = UserProduct.Create("eggs", UserId);
        var userProductCategory = AUserProductCategory("dairy");

        userProductCategory.AddProduct(userProduct);

        Assert.True(userProductCategory.ContainsProductWithGuid(userProduct.Guid));
    }

    private readonly Guid userId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
    private UserProductsManagementService userProductsManagementService;
}
