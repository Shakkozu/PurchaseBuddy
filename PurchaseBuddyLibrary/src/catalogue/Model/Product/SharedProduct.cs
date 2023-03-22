using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;

public class SharedProduct : IProduct
{
	public int Id { get; }
	public Guid Guid { get; }
	public Guid? CategoryId { get; private set; }
	public string Name { get; set; }

	public static SharedProduct CreateNew(string name)
	{
		return new SharedProduct(name, Guid.NewGuid());
	}

	public void AssignProductToCategory(IProductCategory category)
	{
		CategoryId = category.Guid;
	}
	public void RemoveProductCategory()
	{
		CategoryId = null;
	}

	private SharedProduct(string name, Guid guid)
	{
		Name = name;
		Guid = guid;
	}
}
