namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingListItem
{
	public ShoppingListItem(Guid productId, int quantity = 1)
	{
		ProductId = productId;
		Quantity = quantity;
	}

	public void ChangeQuantityTo(int quantity)
	{
		Quantity = quantity;
	}

	public void MarkAsPurchased()
	{
		if (Purchased)
			return;

		Purchased = true;
		Unavailable = false;
	}
	public void MarkAsUnavailable()
	{
		if (Unavailable)
			return;
		Unavailable = true;
		Purchased = false;
	}

	internal void MarkAsNotPurchased()
	{
		if (!Purchased)
			return;

		Purchased = false;
		Unavailable = false;
	}

	public Guid ProductId { get; }
	public int Quantity { get; private set; }
	public bool Purchased { get; private set; }
	public bool Unavailable { get; private set; }
}
