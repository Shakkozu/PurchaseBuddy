using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Category;

public class SharedProductCategory : BaseProductCategory
{
	public static SharedProductCategory CreateNew(string name, string? desc = null)
	{
		return new SharedProductCategory(Guid.NewGuid(), name, desc, null, new List<IProductCategory>(), new List<Guid>());
	}
	public static SharedProductCategory CreateNewWithParent(string name, IProductCategory parent, string? desc = null)
	{
		return new SharedProductCategory(Guid.NewGuid(), name, desc, parent, new List<IProductCategory>(), new List<Guid>());
	}

	public override void SetParent(IProductCategory productCategory)
	{
		if (productCategory is UserProductCategory)
			throw new ArgumentException("User product category cannot be set as parent to shared product category");

		Parent = productCategory;
	}

	public override void AddProduct(IProduct product)
	{
		if (ContainsProductWithGuid(product.Guid))
			return;

		if (children.Any(children => children.ContainsProductWithGuid(product.Guid)))
			return;

		if (Parent != null)
		{
			var parentCategory = Parent;
			while (parentCategory.Parent != null)
				parentCategory = parentCategory.Parent;
			if (parentCategory.ContainsProductWithGuid(product.Guid))
				return;
		}
		if (Parent != null && Parent.ContainsProductWithGuid(product.Guid))
			return;

		productsInCategory.Add(product.Guid);
	}

	private SharedProductCategory(Guid guid, string name, string? description, IProductCategory? parent, List<IProductCategory> children, List<Guid> productsInCategory)
	{
		Guid = guid;
		Name = name;
		Description = description;
		Parent = parent;
		this.children = children;
		this.productsInCategory = productsInCategory;
	}
}
