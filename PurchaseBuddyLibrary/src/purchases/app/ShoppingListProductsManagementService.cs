using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddy.src.purchases.app;

public class ShoppingListProductsManagementService : IShoppingListService
{
	private readonly IShoppingListRepository shoppingListRepository;
	private readonly IProductsRepository userProductsRepository;
	private readonly IUserShopService userShopService;
	private readonly IUserProductsManagementService userProductsManagementService;

	public ShoppingListProductsManagementService(IShoppingListRepository shoppingListRepository,
		IProductsRepository userProductsRepository,
		IUserShopService userShopService,
		IUserProductsManagementService userProductsManagementService)
	{
		this.shoppingListRepository = shoppingListRepository;
		this.userProductsRepository = userProductsRepository;
		this.userShopService = userShopService;
		this.userProductsManagementService = userProductsManagementService;
	}
	public void MarkProductAsUnavailable(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsUnavailable(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void MarkProductAsPurchased(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsPurchased(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void MarkProductAsNotPurchased(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsNotPurchased(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null)
	{
		var list = ShoppingList.CreateNew(userId, listItems, assignedShop);
		shoppingListRepository.Save(list);

		return list.Guid;
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
			if (shop == null)
				throw new ArgumentException("assigned shop not found");

			shopDto = UserShopDto.FromModel(shop);
		}

		var userProducts = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(userId, pageSize: 1000));
		var listItems = shoppingList.Items
			.Select(item => new ShoppingListItemDto(item, userProducts.First(p => p.Guid == item.ProductId)));

		return new ShoppingListDto(userId, shoppingList, shopDto, listItems);
	}

	public IList<ShoppingListDto> GetNotClosedShoppingLists(Guid userId)
	{
		var result = new List<ShoppingListDto>();
		var notCompletedShoppingLists = shoppingListRepository.GetAll(userId).Where(list => !list.IsCompleted);
		if (!notCompletedShoppingLists.Any())
			return result;

		var userShops = userShopService.GetAllUserShops(userId);
		var userProducts = userProductsManagementService.GetUserProducts(new GetUserProductsQuery(userId, pageSize: 1000));
		foreach (var list in notCompletedShoppingLists)
		{
			UserShop? shop = list.ShopId.HasValue ? userShops.First(shop => shop.Guid == list.ShopId.Value) : null;
			var listItems = list.Items
				.Select(item => new ShoppingListItemDto(item, userProducts.First(p => p.Guid == item.ProductId)))
				.ToList();
			result.Add(new ShoppingListDto(userId, list, UserShopDto.FromModel(shop), listItems));
		}

		return result
			.OrderByDescending(list => list.CreatedAt)
			.ToList();
	}

	public void AddProductToList(Guid userId, Guid shoppingListId, UserProduct userProduct)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		var savedProduct = userProductsRepository.GetProduct(userProduct.Guid, userId);
		if (savedProduct is null)
			savedProduct = userProductsRepository.Save(userProduct);

		var shoppingListItem = new ShoppingListItem(savedProduct.Guid);
		shoppingList.AddNew(shoppingListItem);
		shoppingListRepository.Save(shoppingList);
	}
	public void RemoveProductFromList(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.Remove(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid productId, int newQuantity)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.ChangeQuantityOf(productId, newQuantity);
		shoppingListRepository.Save(shoppingList);
	}

	public Guid CreateNewListWithNotBoughtItems(Guid userId, Guid shoppingListId, Guid shopId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		var newShoppingList = shoppingList.GenerateNewWithNotBoughtItems(shopId);
		shoppingListRepository.Save(shoppingList);
		shoppingListRepository.Save(newShoppingList);

		return newShoppingList.Guid;
	}
}
