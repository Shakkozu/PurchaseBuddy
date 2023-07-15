using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.purchases.persistance;

namespace PurchaseBuddyLibrary.purchases.domain;

public class ImportedShoppingListItem : ShoppingListItem
{
	public static ShoppingListItem CreateNew(string productName, string categoryName, int quantity = 1, Guid? guid = null)
	{
		if (quantity < 1)
			throw new ArgumentException("Quantity cannot be lower that 1");

		return new ImportedShoppingListItem(productName, categoryName, quantity, false, false, guid ?? Guid.NewGuid());
	}
	
	public static ImportedShoppingListItem LoadFrom(ImportedShoppingListItemDao dao)
	{
		return new ImportedShoppingListItem(dao.ProductName, dao.CategoryName, dao.Quantity,
			dao.Purchased,
			dao.Unavailable,
			Guid.Parse(dao.Guid));
	}
	private ImportedShoppingListItem(string productName, string categoryName, int quantity, bool purchased, bool unavailable, Guid guid)
		: base(Guid.NewGuid(), quantity, purchased, unavailable, guid)
	{
		ProductName = productName;
		CategoryName = categoryName;
	}

	public string ProductName { get; set; }
    public string CategoryName { get; set; }
}
