using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;

public interface IProduct
{
	public int Id { get; }
	public Guid Guid { get; }
	public string Name { get; set; }
    public Guid? CategoryId { get; }

	void AssignProductToCategory(IProductCategory category);
	void RemoveProductCategory();
}
