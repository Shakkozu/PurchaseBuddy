using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.Tests.stores.unit;
internal class UserShopTests
{
	[Test]
	public void ModifyShopConfiguration_WhenSingleConfigurationEntryIsAdded()
	{
		var dairyCategory = AUserProductCategory("dairy");
		var userShop = AUserShop();
		var categories = new List<Guid> { dairyCategory.Guid };

		userShop.ModifyShopConfiguration(categories);

		Assert.AreEqual(dairyCategory.Guid, userShop.ConfigurationEntries.First().CategoryGuid);
		Assert.AreEqual(1, userShop.ConfigurationEntries.First().Index);
	}

	[Test]
	public void ModifyShopConfiguration_WhenMultipleConfigurationsAreAdded_AssertIndexesAreAssignedInOrder()
	{
		var dairyCategory = AUserProductCategory("dairy");
		var meatCategory = AUserProductCategory("meat");
		var fruitsCategory = AUserProductCategory("fruits");
		var userShop = AUserShop();
		var configurationEntries = new List<Guid> { dairyCategory.Guid, meatCategory.Guid, fruitsCategory.Guid };

		userShop.ModifyShopConfiguration(configurationEntries);

		Assert.AreEqual(dairyCategory.Guid, userShop.ConfigurationEntries.ToArray()[0].CategoryGuid);
		Assert.AreEqual(1, userShop.ConfigurationEntries.ToArray()[0].Index);

		Assert.AreEqual(meatCategory.Guid, userShop.ConfigurationEntries.ToArray()[1].CategoryGuid);
		Assert.AreEqual(2, userShop.ConfigurationEntries.ToArray()[1].Index);

		Assert.AreEqual(fruitsCategory.Guid, userShop.ConfigurationEntries.ToArray()[2].CategoryGuid);
		Assert.AreEqual(3, userShop.ConfigurationEntries.ToArray()[2].Index);
	}
	
	[Test]
	public void WhenDescriptionModified_AssertModifiedCorrectly()
	{
		var userShop = AUserShop();
		userShop.ChangeDescriptionTo(UserShopDescription.CreateNew("name", "desc", null));

		Assert.AreEqual("name", userShop.Description.Name);
		Assert.AreEqual("desc", userShop.Description.Description);
		Assert.Null(userShop.Description.Address);
	}

	private UserShop AUserShop()
	{
		return UserShop.CreateNew(Fixture.UserId, UserShopDescription.CreateNew("shop1", null, null));
	}

	public UserProductCategory AUserProductCategory(string name)
	{
		return UserProductCategory.CreateNew(name, Fixture.UserId);
	}
}
