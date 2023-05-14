using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingListItem
{
	public static ShoppingListItem CreateNew(Guid productId, int quantity = 1)
	{
		return new ShoppingListItem(productId, quantity, false, false);
	}

	internal static ShoppingListItem LoadFrom(ShoppingListDao.ShoppingListItemDao item)
	{
		return new ShoppingListItem(Guid.Parse(item.ItemGuid), item.Quantity, item.Purchased, item.Unavailable);
	}

    private ShoppingListItem(Guid productId, int quantity, bool purchased, bool unavailable)
    {
		ProductId = productId;
		Quantity = quantity;
		Purchased = purchased;
		Unavailable = unavailable;
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
