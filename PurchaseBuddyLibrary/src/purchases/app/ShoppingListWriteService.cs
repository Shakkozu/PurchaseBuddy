using MediatR;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.app.events;

namespace PurchaseBuddy.src.purchases.app;

internal class ShoppingListWriteService : IShoppingListWriteService
{
	private readonly IShoppingListRepository shoppingListRepository;
	private readonly IUserProductsManagementService userProductsManagementService;
	private readonly IMediator mediator;

	public ShoppingListWriteService(IShoppingListRepository shoppingListRepository,
        IUserProductsManagementService userProductsManagementService,
		IMediator mediator)
    {
		this.shoppingListRepository = shoppingListRepository;
		this.userProductsManagementService = userProductsManagementService;
		this.mediator = mediator;
	}

	public void MarkListItemtAsUnavailable(Guid userId, Guid shoppingListId, Guid listItemId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"Shopping list with id {shoppingListId} not found for user {userId}");

		shoppingList.MarkListItemAsUnavailable(listItemId);
		shoppingListRepository.Update(shoppingList);
	}
	public void MarkListItemAsPurchased(Guid userId, Guid shoppingListId, Guid listItemId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"Shopping list with id {shoppingListId} not found for user {userId}");

		shoppingList.MarkListItemAsPurchased(listItemId);
		if (shoppingList.IsCompleted)
			mediator.Send(new ShoppingCompleted(shoppingList.Guid, shoppingList.UserId));

		shoppingListRepository.Update(shoppingList);
	}
	public void MarkListItemAsNotPurchased(Guid userId, Guid shoppingListId, Guid listItemId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"Shopping list with id {shoppingListId} not found for user {userId}");

		shoppingList.MarkListItemAsNotPurchased(listItemId);
		shoppingListRepository.Update(shoppingList);
	}
	public Guid CreateNewList(Guid userId, List<ShoppingListItem> listItems, Guid? assignedShop = null)
	{
		var list = ShoppingList.CreateNew(userId, listItems, assignedShop);
		shoppingListRepository.Save(list);

		return list.Guid;
	}
	public void RemoveItemFromList(Guid userId, Guid shoppingListId, Guid listItemId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		if (shoppingList == null)
			throw new ArgumentException($"Shopping list with id {shoppingListId} not found for user {userId}");

		shoppingList.Remove(listItemId, userId);
		shoppingListRepository.Update(shoppingList);
	}
	public void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid listItemId, int newQuantity)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.ChangeQuantityOf(listItemId, newQuantity, userId);
		shoppingListRepository.Update(shoppingList);
	}

	public void AddNewListItem(Guid userId, Guid listId, AddNewListItemRequest addNewItemRequest)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, listId);
		if(shoppingList == null)
			throw new ArgumentNullException($"Shopping list with guid {listId} not found for user {userId}");

		var listItem = CreateShoppingListItemFromRequest(userId, addNewItemRequest, shoppingList.UserId == userId);
        shoppingList.AddNew(listItem, userId);
		shoppingListRepository.Update(shoppingList);
	}
    
    private ShoppingListItem CreateShoppingListItemFromRequest(Guid userId, AddNewListItemRequest addNewItemRequest, bool isUserListCreator)
    {
        var quantity = addNewItemRequest.Quantity.GetValueOrDefault(1);
        if (addNewItemRequest.ProductGuid.HasValue)
        {
            var userProducts =
                userProductsManagementService.GetUserProducts(new GetUserProductsQuery(userId, pageSize: 1000));
            var productToAdd = userProducts.Find(p => p.Guid == addNewItemRequest.ProductGuid);
            if (productToAdd == null)
                throw new ArgumentNullException(
                    $"Product with guid {addNewItemRequest.ProductGuid} not found for user {userId}");

			if (!isUserListCreator)
				return ImportedShoppingListItem.CreateNew(productToAdd.Name, productToAdd.CategoryName, quantity, addNewItemRequest.ListItemGuid);

			return ShoppingListItem.CreateNew(productToAdd.Guid, quantity, addNewItemRequest.ListItemGuid);
        }

        if (string.IsNullOrEmpty(addNewItemRequest.ProductName))
            throw new ArgumentException($"Cannot add product without guid and without name defined");

        return ImportedShoppingListItem.CreateNew(addNewItemRequest.ProductName,
            addNewItemRequest.ProductCategoryName, quantity, addNewItemRequest.ListItemGuid);
    }

	public void GrantAccessToModifyingList(Guid listId, Guid userId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(listId);
		if (shoppingList == null)
			throw new ArgumentNullException($"Shopping list with guid {listId} not found");

		shoppingList.GrantAccessToModifyingTo(userId);
		shoppingListRepository.Update(shoppingList);
	}
}
