using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;

public class ShoppingListItemDto
{
    public ShoppingListItemDto(ShoppingListItem shoppingListItem, UserProductDto productDto)
    {
        Quantity = shoppingListItem.Quantity;
        ProductDto = productDto;
        Purchased = shoppingListItem.Purchased;
        Unavailable = shoppingListItem.Unavailable;
    }
    public int Quantity { get; set; }
    public int Index { get; set; }
    public UserProductDto ProductDto { get; set; }
    public bool Purchased { get; set; }
    public bool Unavailable { get; set; }

    public override bool Equals(object? obj)
    {
        var other = obj as ShoppingListItemDto;
        if (other == null)
            return false;
        return other.Quantity == Quantity &&
            other.ProductDto.Equals(ProductDto);
    }
}

