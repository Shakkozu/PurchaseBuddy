using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;
using PurchaseBuddyLibrary.src.crm;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddy.src.purchases.app;

public class ShoppingListReadService : IShoppingListReadService
{
	private readonly IShoppingListRepository shoppingListRepository;
	private readonly IUserShopService userShopService;
	private readonly IUserProductsManagementService userProductsManagementService;
	private readonly IUsersProvider usersProvider;

	public ShoppingListReadService(IUserShopService userShopService,
								IShoppingListRepository shoppingListRepository,
								IUserProductsManagementService userProductsManagementService,
								IUsersProvider usersProvider)
	{
		this.userShopService = userShopService;
		this.shoppingListRepository = shoppingListRepository;
		this.userProductsManagementService = userProductsManagementService;
		this.usersProvider = usersProvider;
	}
	public ShoppingListDto GetShoppingList(Guid userId, Guid shoppingListId)
	{
		var users = usersProvider.GetAllUsers();
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"shopping list with id {shoppingListId} not found for user {userId}");

		UserShopDto? shopDto = null;
		if (shoppingList.ShopId.HasValue)
		{
			var shop = userShopService.GetUserShopById(shoppingList.UserId, shoppingList.ShopId.Value);
			if (shop != null)
				shopDto = UserShopDto.FromModel(shop);
		}

		var userProducts = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(shoppingList.UserId, pageSize: 1000));
		var user = users.Single(u => u.Guid == userId);
		List<ShoppingListItemDto> listItems = new List<ShoppingListItemDto>();
		foreach (var item in shoppingList.Items)
			AddShoppingListItemDtoToList(item, listItems, userProducts);

		return new ShoppingListDto(user, shoppingList, shopDto, listItems);
	}

	public IList<ShoppingListDto> GetAllShoppingLists(Guid userId)
	{
		var result = new List<ShoppingListDto>();
		var users = usersProvider.GetAllUsers();
		var shoppingLists = shoppingListRepository.GetAll(userId);
		if (!shoppingLists.Any())
			return result;

		var listsOwners = shoppingLists.Select(x => x.UserId).Distinct().ToList();
		Dictionary<Guid, List<UserProductDto>> ownersProducts = new Dictionary<Guid, List<UserProductDto>>();
		if (listsOwners.Any())
		{
			foreach(var  creator in listsOwners)
				ownersProducts[creator] = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(creator, pageSize: 1000));
		}
		var userShops = userShopService.GetAllUserShops(userId);
		foreach (var list in shoppingLists)
		{
			var user = users.Single(u => u.Guid == list.UserId);
			var creatorProducts = ownersProducts[list.UserId];
			UserShop? shop = list.ShopId.HasValue ? userShops.Find(shop => shop.Guid == list.ShopId.Value) : null;
            var listItems = new List<ShoppingListItemDto>();
            foreach (var item in list.Items)
                AddShoppingListItemDtoToList(item, listItems, creatorProducts);
			result.Add(new ShoppingListDto(user, list, UserShopDto.FromModel(shop), listItems));
		}

		return result
			.OrderByDescending(list => list.CreatedAt)
			.ToList();
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
}
