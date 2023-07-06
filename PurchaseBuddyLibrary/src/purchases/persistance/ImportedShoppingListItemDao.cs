using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.purchases.persistance;

public class ImportedShoppingListItemDao : ShoppingListItemDao
{
    public ImportedShoppingListItemDao()
    {
        
    }
    public ImportedShoppingListItemDao(ImportedShoppingListItem item)
    {
		Guid = item.Guid.ToDatabaseStringFormat();
		Quantity = item.Quantity;
		Purchased = item.Purchased;
		Unavailable = item.Unavailable;
		ProductName = item.ProductName;
		CategoryName = item.CategoryName;
	}
	public string ProductName { get; set; }
	public string CategoryName { get; set; }
	public override string Type { get; set; } = ShoppingListItemTypes.Imported;
}
