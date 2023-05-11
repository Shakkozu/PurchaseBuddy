using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProductCategories;

public class ProductCategoryDto
{
    public ProductCategoryDto(IProductCategory productCategory)
    {
        // load children recursively
        var children = GetChildrenRecursively(productCategory);

        Guid = productCategory.Guid;
        Name = productCategory.Name;
        Description = productCategory.Description;
        IsRoot = productCategory.IsRoot;
        ParentId = productCategory.ParentId;
        Children = children;
    }

    private static List<ProductCategoryDto> GetChildrenRecursively(IProductCategory productCategory)
    {
        var children = new List<ProductCategoryDto>();
        foreach (var child in productCategory.Children)
        {
            children.Add(new ProductCategoryDto(child));
        }

        return children;
    }

    public Guid Guid { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool IsRoot { get; set; }
    public Guid? ParentId { get; set; }
    public IReadOnlyCollection<ProductCategoryDto> Children { get; }
}


