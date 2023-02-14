namespace PurchaseBuddy.src.catalogue.Model;

public class ProductCategory : BaseProductCategory
{
	public static ProductCategory CreateNew(string name, string? desc = null)
	{
		return new ProductCategory(Guid.NewGuid(),  name, desc, null, new List<IProductCategory>(), new List<Guid>());
	}
	public static ProductCategory CreateNewWithParent(string name, IProductCategory parent, string? desc = null)
	{
		return new ProductCategory(Guid.NewGuid(), name, desc, parent, new List<IProductCategory>(), new List<Guid>());
	}

	public override void SetParent(IProductCategory productCategory)
	{
		if (productCategory is UserProductCategory)
			throw new ArgumentException("User product category cannot be set as parent to shared product category");

		Parent = productCategory;
	}

	private ProductCategory(Guid guid, string name, string? description, IProductCategory? parent, List<IProductCategory> children, List<Guid> productsInCategory)
	{
		Guid = guid;
		Name = name;
		Description = description;
		Parent = parent;
		this.children = children;
		this.productsInCategory = productsInCategory;
	}
}
