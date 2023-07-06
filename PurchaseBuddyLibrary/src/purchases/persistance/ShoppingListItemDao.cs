namespace PurchaseBuddyLibrary.purchases.persistance;

public abstract class ShoppingListItemDao
{
	public abstract string Type { get; set; }
	public int Quantity { get; set; }
	public bool Purchased { get; set; }
	public bool Unavailable { get; set; }
	public string Guid { get; set; }
}
