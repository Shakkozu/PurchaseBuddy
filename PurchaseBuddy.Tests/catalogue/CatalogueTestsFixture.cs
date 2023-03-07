using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.Tests.catalogue;

[TestFixture]
internal class CatalogueTestsFixture
{
	protected UserProductCategory AUserProductCategory(string? name = null)
	{
		return UserProductCategory.CreateNew(name ?? "dairy", UserId);
	}
	protected CreateUserCategoryRequest AUserProductCategoryCreateRequest(string? name = null, Guid? parentId = null)
	{
		return new CreateUserCategoryRequest(name ?? "dairy", null, parentId);
	}

	public SharedProductCategory ASharedCategory(string name, IProductCategory? parent = null)
	{
		return parent != null
			? SharedProductCategory.CreateNewWithParent(name, parent)
			: SharedProductCategory.CreateNew(name);
	}


	protected UserProductCategory AUserProductCategoryWithParent(UserProductCategory parent, string? name = null)
	{
		var child = UserProductCategory.CreateNewWithParent(name ?? "eggs", UserId, parent);
		parent.AddChild(child);

		return child;
	}

	protected Guid UserId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
}