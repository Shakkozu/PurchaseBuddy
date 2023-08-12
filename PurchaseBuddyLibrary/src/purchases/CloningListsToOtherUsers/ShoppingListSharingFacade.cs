using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddyLibrary.src.purchases.CloningListsToOtherUsers;

public class ShoppingListSharingFacade
{
	private readonly IShoppingListReadService shoppingListReadService;
	private readonly ISharedShoppingListRepository shoppingListRepository;
	private readonly IShoppingListWriteService shoppingListWriteService;

	public ShoppingListSharingFacade(IShoppingListReadService shoppingListReadService,
		ISharedShoppingListRepository shoppingListRepository,
		IShoppingListWriteService shoppingListWriteService
	)
	{
		this.shoppingListReadService = shoppingListReadService;
		this.shoppingListRepository = shoppingListRepository;
		this.shoppingListWriteService = shoppingListWriteService;
	}

	public Guid CreateSharedList(Guid userId, Guid listId)
	{
		var listToShare = shoppingListReadService.GetShoppingList(userId, listId);
		if (listToShare == null || listToShare.CompletedAt.HasValue)
			throw new ArgumentException($"List with id {listId} not found for user {userId}");

		var products = listToShare.ShoppingListItems.Select(listItem => listItem.ProductDto);
		var listItems = products
			.Select(product => new SharedListItem(product.Name, product.CategoryName))
			.ToList();
		var list = new SharedList(userId, listItems, listId);
		var alreadyCreatedListWithSameSource = shoppingListRepository
			.GetAllWithSourceAndCreator(listId, userId)
			.Find(alreadyCreatedList => alreadyCreatedList.Items.SequenceEqual(list.ToDto().Items));
		if (alreadyCreatedListWithSameSource != null)
			return alreadyCreatedListWithSameSource.Guid;

		shoppingListRepository.Save(list.ToDto());
		return list.Guid;
	}

	public SharedListDto GetSharedList(Guid listToShareId)
	{
		var list = shoppingListRepository.Get(listToShareId);
		if (list == null)
			throw new ArgumentException($"List with id {listToShareId} not found");

		return list;
	}

	public Guid ImportSharedList(Guid userId, Guid sharedListId)
	{
		var list = shoppingListRepository.Get(sharedListId);
		if (list == null)
			throw new ArgumentException($"List with id {sharedListId} not found");
		if (list.CreatorId == userId)
			throw new InvalidOperationException("Creator cannot import shopping alreadyCreatedList");

		var listItems =
			list.Items.Select(item => ImportedShoppingListItem.CreateNew(item.productName, item.categoryName))
			.ToList();

		return shoppingListWriteService.CreateNewList(userId, listItems);
	}
}
