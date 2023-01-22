using System.Collections.ObjectModel;

namespace PurchaseBuddy.src.catalogue.Model;

public class UserProductCategory
{
	public Guid Guid { get; private set; }
	public Guid UserId { get; private set; }
	public string Name { get; private set; }
	public string? Description { get; private set; }
	public UserProductCategory? Parent { get; private set; }
	public IReadOnlyCollection<UserProductCategory> Children { get => new ReadOnlyCollection<UserProductCategory>(children); }

	public static UserProductCategory CreateNew(string name, Guid userId, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, null, new List<UserProductCategory>(), new List<Guid>());
	}
	public static UserProductCategory CreateNewWithParent(string name, Guid userId, UserProductCategory parent, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, null, new List<UserProductCategory>(), new List<Guid>());
	}
	
	public void AddChild(UserProductCategory child)
	{
		children.Add(child);
		child.Parent = this;
	}

	private UserProductCategory(Guid guid, Guid userId, string name, string? description, UserProductCategory? parent, List<UserProductCategory> children, List<Guid> productsInCategory)
	{
		Guid = guid;
		UserId = userId;
		Name = name;
		Description = description;
		Parent = parent;
		this.children = children;
		this.productsInCategory = productsInCategory;
	}

	public List<Guid> GetProductsInCategory()
	{
		if (children == null || !children.Any())
			return productsInCategory;

		var result = new List<Guid>(productsInCategory);
		foreach (var child in children)
		{
			result.AddRange(child.GetProductsInCategory());
		}

		return result;
	}

	public void AddProduct(UserProduct userProduct)
	{
		if (userProduct.UserID != UserId)
			throw new ArgumentException("Cannot add product do different user category");

		if (ContainsProductWithGuid(userProduct.Guid))
			return;

		if (children.Any(children => children.ContainsProductWithGuid(userProduct.Guid)))
			return;

		if(Parent != null)
		{
			var parentCategory = Parent;
			while(parentCategory.Parent != null)
				parentCategory = parentCategory.Parent;
			if (parentCategory.ContainsProductWithGuid(userProduct.Guid))
				return;
		}
		if (Parent != null && Parent.ContainsProductWithGuid(userProduct.Guid))
			return;

		productsInCategory.Add(userProduct.Guid);
	}

	public void RemoveProduct(UserProduct userProduct)
	{
		if (!productsInCategory.Contains(userProduct.Guid))
			return;

		// todo: If it's possible to fetch products from children categories, removing a product from child via root should be possible

		productsInCategory.Remove(userProduct.Guid);
	}

	public bool ContainsProductWithGuid(Guid userProductId)
	{
		return GetProductsInCategory().Any(productId => productId == userProductId);
	}

	private readonly List<Guid> productsInCategory;
	private readonly List<UserProductCategory> children;
}
