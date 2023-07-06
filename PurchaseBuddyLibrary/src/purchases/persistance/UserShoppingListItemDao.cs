using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.purchases.persistance;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.src.purchases.persistance;

public class UserShoppingListItemDao : ShoppingListItemDao
{
	public UserShoppingListItemDao()
	{

	}
	public UserShoppingListItemDao(ShoppingListItem item)
	{
		ItemGuid = item.ProductId.ToDatabaseStringFormat();
		Quantity = item.Quantity;
		Purchased = item.Purchased;
		Unavailable = item.Unavailable;
		Guid = item.Guid.ToDatabaseStringFormat();
	}
	public string ItemGuid { get; set; }
	public override string Type { get; set; } = ShoppingListItemTypes.UserDefined;
}
