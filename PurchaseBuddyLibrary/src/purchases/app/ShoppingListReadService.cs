using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddy.src.purchases.app;

public class ShoppingListReadService : IShoppingListReadService
{
	private readonly IShoppingListRepository shoppingListRepository;
	private readonly IUserShopService userShopService;
	private readonly IUserProductsManagementService userProductsManagementService;

	public ShoppingListReadService(IUserShopService userShopService,
								IShoppingListRepository shoppingListRepository,
								IUserProductsManagementService userProductsManagementService)
	{
		this.userShopService = userShopService;
		this.shoppingListRepository = shoppingListRepository;
		this.userProductsManagementService = userProductsManagementService;
	}
	public ShoppingListDto GetShoppingList(Guid userId, Guid shoppingListId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"shopping list with id {shoppingListId} not found for user {userId}");

		UserShopDto? shopDto = null;
		if (shoppingList.ShopId.HasValue)
		{
			var shop = userShopService.GetUserShopById(userId, shoppingList.ShopId.Value);
			if (shop != null)
				shopDto = UserShopDto.FromModel(shop);
		}

		var userProducts = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(userId, pageSize: 1000));
		List<ShoppingListItemDto> listItems = new List<ShoppingListItemDto>();
		foreach(var item in shoppingList.Items)
        {
            AddShoppingListItemDtoToList(item, listItems, userProducts);
        }

		return new ShoppingListDto(userId, shoppingList, shopDto, listItems);
	}

    private static void AddShoppingListItemDtoToList(ShoppingListItem item, List<ShoppingListItemDto> listItems, List<UserProductDto> userProducts)
    {
        if (item is ImportedShoppingListItem importedItem)
        {
            listItems.Add(new ShoppingListItemDto(importedItem));
            return;
        }

        listItems.Add(new ShoppingListItemDto(item, userProducts.First(p => p.Guid == item.ProductId)));
    }

    public IList<ShoppingListDto> GetAllShoppingLists(Guid userId)
	{
		var result = new List<ShoppingListDto>();
		var notCompletedShoppingLists = shoppingListRepository.GetAll(userId);
		if (!notCompletedShoppingLists.Any())
			return result;

		var userShops = userShopService.GetAllUserShops(userId);
		var userProducts = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(userId, pageSize: 1000));
		foreach (var list in notCompletedShoppingLists)
		{
			UserShop? shop = list.ShopId.HasValue ? userShops.FirstOrDefault(shop => shop.Guid == list.ShopId.Value) : null;
            var listItems = new List<ShoppingListItemDto>();
            foreach (var item in list.Items)
                AddShoppingListItemDtoToList(item, listItems, userProducts);
			result.Add(new ShoppingListDto(userId, list, UserShopDto.FromModel(shop), listItems));
		}

		return result
			.OrderByDescending(list => list.CreatedAt)
			.ToList();
	}
}
