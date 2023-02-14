namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;

public interface IProduct
{
    public int Id { get; }
    public Guid Guid { get; }
	public string Name { get; }
}
