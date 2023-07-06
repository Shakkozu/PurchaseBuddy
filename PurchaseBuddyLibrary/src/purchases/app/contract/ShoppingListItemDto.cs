using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;

public class AddNewListItemRequest
{
	public string? ProductName { get; set; }
	public string? ProductCategoryName { get; set; }
	public Guid? ProductGuid { get; set; }
	public Guid ListItemGuid { get; set; }
	public int? Quantity { get; set; }
}
public class ShoppingListItemDto
{
    public ShoppingListItemDto()
    {
        
    }
    public ShoppingListItemDto(ImportedShoppingListItem importedItem)
	{
		Quantity = importedItem.Quantity;
		ProductDto = new UserProductDto
		{
			CategoryName = importedItem.CategoryName,
			Name = importedItem.ProductName
		};
		Purchased = importedItem.Purchased;
		Unavailable = importedItem.Unavailable;
		Guid = importedItem.Guid.ToString();
	}

	public ShoppingListItemDto(ShoppingListItem shoppingListItem, UserProductDto productDto)
    {
        Quantity = shoppingListItem.Quantity;
        ProductDto = productDto;
        Purchased = shoppingListItem.Purchased;
        Unavailable = shoppingListItem.Unavailable;
		Guid = shoppingListItem.Guid.ToString();
	}
    public int Quantity { get; set; }
    public int Index { get; set; }
    public UserProductDto ProductDto { get; set; }
    public bool Purchased { get; set; }
    public bool Unavailable { get; set; }
    public string Guid { get; set; }

    public override bool Equals(object? obj)
    {
        var other = obj as ShoppingListItemDto;
        if (other == null)
            return false;
        return other.Quantity == Quantity &&
            other.ProductDto.Equals(ProductDto);
    }
}

