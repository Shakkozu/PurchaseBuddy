using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.purchases.persistance;

namespace PurchaseBuddy.src.purchases.persistance;

public static class ShoppingListItemMapper
{
	public static List<ShoppingListItemDao> Map(IEnumerable<ShoppingListItem> list)
	{
		var result = new List<ShoppingListItemDao>();
		foreach(var item in list)
		{
			if (item is ImportedShoppingListItem importedItem)
			{
				result.Add(new ImportedShoppingListItemDao(importedItem));
				continue;
			}
			result.Add(new UserShoppingListItemDao(item));
		}

		return result;
	}
}
