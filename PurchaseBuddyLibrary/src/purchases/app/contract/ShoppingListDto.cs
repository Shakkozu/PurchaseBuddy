using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.crm;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddyLibrary.src.purchases.app.contract;
public class ShoppingListDto
{
    public ShoppingListDto(UserDto userDto, ShoppingList shoppingList, UserShopDto? shopDto, IEnumerable<ShoppingListItemDto> listItems)
    {
        CreatorId = shoppingList.UserId;
        CreatorName = userDto.Name;
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
		var index = 1;
        foreach (var categoryGuid in categoriesMap)
		{
			var itemsToAdd = listItems.Where(item => item.ProductDto.CategoryId == categoryGuid);
			AddItems(itemsToAdd, result, ref index);
		}

		var itemsWithoutCategories = listItems.Where(listItem => !categoriesMap.Any(mapEntry => mapEntry == listItem.ProductDto.CategoryId));
		AddItems(itemsWithoutCategories, result, ref index);

        return result;
    }

	private static void AddItems(IEnumerable<ShoppingListItemDto> itemsToAdd, List<ShoppingListItemDto> result, ref int index)
	{
		foreach (var item in itemsToAdd)
		{
			item.Index = index++;
			result.Add(item);
		}
	}

	public Guid Guid { get; set; }
    public Guid CreatorId { get; set; }
	public string CreatorName { get; }
	public UserShopDto? AssignedShop { get; set; }
	
	public List<ShoppingListItemDto> ShoppingListItems { get; set; }
	public bool? Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

