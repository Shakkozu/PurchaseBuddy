namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class SharedProductCategoryTests : CatalogueTestsFixture
{
	[Test]
	public void UserCategoryCannotBeSetAsParentToSharedCategory()
	{
		var parent = AUserProductCategory("dairy");
		var child = ASharedCategory("milk");

		Assert.Throws<ArgumentException>(() => child.SetParent(parent));
	}
	
	[Test]
	public void SharedCategoryCanBeAddedAsChildToSharedCategory()
	{
		var parent = ASharedCategory("dairy");
		var child = ASharedCategory("milk");

		parent.AddChild(child);

		Assert.AreEqual(parent.Children.First(), child);
		Assert.AreEqual(child.Parent, parent);
	}

	[Test]
	public void UserCategoryCanBeAddedAsChildToSharedCategory()
	{
		var parent = ASharedCategory("dairy");
		var child = AUserProductCategory("milk");

		parent.AddChild(child);

		Assert.AreEqual(parent.Children.First(), child);
		Assert.AreEqual(child.Parent, parent);
	}
}
