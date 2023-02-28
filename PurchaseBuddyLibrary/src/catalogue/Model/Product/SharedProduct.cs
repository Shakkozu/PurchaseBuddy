namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;

public class SharedProduct : IProduct
{
	public int Id { get; }
	public Guid Guid { get; }
	public string Name { get; }

	public static SharedProduct CreateNew(string name)
	{
		return new SharedProduct(name, Guid.NewGuid());
	}
	private SharedProduct(string name, Guid guid)
	{
		Name = name;
		Guid = guid;
	}
}
