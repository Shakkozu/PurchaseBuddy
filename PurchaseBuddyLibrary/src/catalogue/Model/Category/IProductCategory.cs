namespace PurchaseBuddyLibrary.src.catalogue.Model.Category;

public interface IProductCategory
{
    public Guid Guid { get; }
    public IProductCategory? Parent { get; }
    public IReadOnlyCollection<IProductCategory> Children { get; }
    bool ContainsProductWithGuid(Guid guid);
    List<Guid> GetProductsInCategory();
    void AddChild(IProductCategory productCategory);
    void SetParent(IProductCategory productCategory);
}
