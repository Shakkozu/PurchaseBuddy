using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;
public class ShoppingListDto
{
    public ShoppingListDto(Guid userId, ShoppingList shoppingList, UserShopDto? shopDto, IEnumerable<ShoppingListItemDto> listItems)
    {
        UserId = userId;
        Guid = shoppingList.Guid;
        AssignedShop = shopDto;
        Completed = shoppingList.CompletedAt.HasValue;
        CompletedAt = shoppingList.CompletedAt;
        CreatedAt = shoppingList.CreatedAt;

        if (shopDto != null && shopDto.CategoriesMap.Any())
            ShoppingListItems = SortListItemsByShopConfiguration(shopDto.CategoriesMap, listItems);
        else
            ShoppingListItems = listItems.ToList();
    }

    private List<ShoppingListItemDto> SortListItemsByShopConfiguration(List<Guid> categoriesMap, IEnumerable<ShoppingListItemDto> listItems)
    {
        var result = new List<ShoppingListItemDto>();
        foreach (var categoryGuid in categoriesMap)
            result.AddRange(listItems.Where(item => item.ProductDto.CategoryId == categoryGuid));

		result.AddRange(listItems.Where(listItem => !categoriesMap.Any(mapEntry => mapEntry == listItem.ProductDto.CategoryId)));

        return result;
    }

    public Guid Guid { get; set; }
    public Guid UserId { get; set; }
    public UserShopDto? AssignedShop { get; set; }

    public List<ShoppingListItemDto> ShoppingListItems { get; set; } = new List<ShoppingListItemDto> { };
    public bool? Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

