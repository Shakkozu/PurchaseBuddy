namespace PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

public interface ISharedShoppingListRepository
{
	void Save(SharedListDto list);
	SharedListDto? Get(Guid listToShareId);
	List<SharedListDto> GetAllWithSourceAndCreator(Guid listId, Guid userId);
}
