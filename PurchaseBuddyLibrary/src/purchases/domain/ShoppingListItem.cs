using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddyLibrary.purchases.domain;

public class ShoppingListItem
{
	public static ShoppingListItem CreateNew(Guid productId, int quantity = 1, Guid? guid = null)
	{
		if (quantity < 1)
			throw new ArgumentException("Quantity cannot be lower that 1");
		return new ShoppingListItem(productId, quantity, false, false, guid ?? Guid.NewGuid());
	}

	internal static ShoppingListItem LoadFrom(UserShoppingListItemDao item)
    {
        var guid = string.IsNullOrEmpty(item.Guid) ? Guid.NewGuid() : Guid.Parse(item.Guid);
		return new ShoppingListItem(Guid.Parse(item.ItemGuid), item.Quantity, item.Purchased, item.Unavailable, guid);
	}

    protected ShoppingListItem(Guid productId, int quantity, bool purchased, bool unavailable, Guid guid)
    {
		ProductId = productId;
		Quantity = quantity;
		Purchased = purchased;
		Unavailable = unavailable;
		Guid = guid;
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
	public Guid Guid { get; }
	public int Quantity { get; private set; }
	public bool Purchased { get; private set; }
	public bool Unavailable { get; private set; }
}
