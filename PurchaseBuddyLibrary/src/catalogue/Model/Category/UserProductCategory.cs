using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Category;
public class UserProductCategory : BaseProductCategory
{
	public Guid UserId { get; private set; }
	public static UserProductCategory CreateNew(string name, Guid userId, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, null, new List<IProductCategory>(), new List<Guid>());
	}
	public static UserProductCategory CreateNewWithParent(string name, Guid userId, IProductCategory? parent, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, parent, new List<IProductCategory>(), new List<Guid>());
	}

	private UserProductCategory(Guid guid, Guid userId, string name, string? description, IProductCategory? parent, List<IProductCategory> children, List<Guid> productsInCategory)
	{
		Guid = guid;
		UserId = userId;
		Name = name;
		Description = description;
		this.children = children;
		this.productsInCategory = productsInCategory;
		if (parent != null)
		{
			ParentId = parent.Guid;
			parent.AddChild(this);
		}
	}

	public override void AddProduct(IProduct product)
	{
		var userProduct = product as UserProduct;
		if (userProduct != null && userProduct.UserID != UserId)
			throw new ArgumentException("Cannot add product do different user category");

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

	public void RemoveProduct(IProduct product)
	{
		if (!productsInCategory.Contains(product.Guid))
			return;

		productsInCategory.Remove(product.Guid);
	}

	internal static IProductCategory LoadFrom(ProductCategoryDao pcd, ProductCategoryDao? parentCategory = null, List<ProductCategoryDao>? children = null)
	{
		//var parent = 
		return new UserProductCategory(
			Guid.Parse(pcd.Guid),
			Guid.Parse(pcd.UserGuid),
			pcd.Name,
			pcd.Description,
			null,
			null,
			null);
	}

	internal static IProductCategory LoadFrom(int id, Guid guid, Guid userGuid, string name, string description)
	{
		return new UserProductCategory(
			guid,
			userGuid,
			name,
			description,
			null,
			new List<IProductCategory>(),
			new List<Guid>());
	}
}
