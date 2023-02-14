namespace PurchaseBuddy.src.catalogue.Model;
public class UserProductCategory : BaseProductCategory
{
	public Guid UserId { get; private set; }

	public static UserProductCategory CreateNew(string name, Guid userId, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, null, new List<IProductCategory>(), new List<Guid>());
	}
	public static UserProductCategory CreateNewWithParent(string name, Guid userId, IProductCategory parent, string? desc = null)
	{
		return new UserProductCategory(Guid.NewGuid(), userId, name, desc, parent, new List<IProductCategory>(), new List<Guid>());
	}

	private UserProductCategory(Guid guid, Guid userId, string name, string? description, IProductCategory? parent, List<IProductCategory> children, List<Guid> productsInCategory)
	{
		Guid = guid;
		UserId = userId;
		Name = name;
		Description = description;
		Parent = parent;
		this.children = children;
		this.productsInCategory = productsInCategory;
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

	public override void SetParent(IProductCategory productCategory)
	{
		Parent = productCategory;
	}
}
