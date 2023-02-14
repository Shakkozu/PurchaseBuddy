using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Category;
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

        // todo: If it's possible to fetch products from children categories, removing a product from child via root should be possible
        productsInCategory.Remove(product.Guid);
    }

    public override void SetParent(IProductCategory productCategory)
    {
        Parent = productCategory;
    }
}
