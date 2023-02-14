using PurchaseBuddy.src.catalogue.Model;

namespace PurchaseBuddy.Tests.catalogue;

[TestFixture]
internal class CatalogueTestsFixture
{
	protected UserProductCategory AUserProductCategory(string? name = null)
	{
		return UserProductCategory.CreateNew(name ?? "dairy", UserId);
	}

	public ProductCategory ASharedCategory(string name)
	{
		return ProductCategory.CreateNew(name);
	}


	protected UserProductCategory AUserProductCategoryWithParent(UserProductCategory parent, string? name = null)
	{
		var child = UserProductCategory.CreateNewWithParent(name ?? "eggs", UserId, parent);
		parent.AddChild(child);

		return child;
	}

	protected Guid UserId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
}