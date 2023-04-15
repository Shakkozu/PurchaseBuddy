using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.Tests.stores.unit;
internal class UserShopTests
{
	[Test]
	public void WhenDescriptionModified_AssertModifiedCorrectly()
	{
		var userShop = AUserShop();
		userShop.ChangeDescriptionTo(UserShopDescription.CreateNew("name", "desc", null));

		Assert.AreEqual("name", userShop.Description.Name);
		Assert.AreEqual("desc", userShop.Description.Description);
		Assert.Null(userShop.Description.Address);
	}
	
	[Test]
	public void AssertCOnfigurationModificationWorksCorrectly()
	{
		var g1 = UserProductCategory.CreateNew("test2", Fixture.UserId);
		var g2 = UserProductCategory.CreateNew("test", Fixture.UserId);
		var productCategories = new List<IProductCategory> { g1, g2 };
		var userShop = AUserShop();

		userShop.ModifyShopConfiguration(productCategories);

		Assert.AreEqual(g1.Guid, userShop.ConfigurationEntries.First().CategoryGuid);
		Assert.AreEqual(1, userShop.ConfigurationEntries.First().Index);
		Assert.AreEqual(g2.Guid, userShop.ConfigurationEntries.Last().CategoryGuid);
		Assert.AreEqual(2, userShop.ConfigurationEntries.Last().Index);
		Assert.AreEqual(2, userShop.ConfigurationEntries.Count);
	}

	private UserShop AUserShop(List<IProductCategory>? productCategories = null)
	{
		var guids = new[] {  Guid.NewGuid() };
		if(productCategories == null)
			productCategories = new List<IProductCategory> { UserProductCategory.CreateNew("testName", Fixture.UserId) };
		return UserShop.CreateNew(Fixture.UserId, UserShopDescription.CreateNew("shop1", null, null), productCategories);
	}

	public UserProductCategory AUserProductCategory(string name)
	{
		return UserProductCategory.CreateNew(name, Fixture.UserId);
	}
}
