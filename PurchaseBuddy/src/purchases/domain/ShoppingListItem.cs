namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingListItem
{
	public ShoppingListItem(Guid productId, int quantity = 1)
	{
		ProductId = productId;
		Quantity = quantity;
	}

	public Guid ProductId { get; }
	public int Quantity { get; }
}
