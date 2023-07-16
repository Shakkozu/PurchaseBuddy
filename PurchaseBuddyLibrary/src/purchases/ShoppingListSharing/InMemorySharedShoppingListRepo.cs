using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

public class InMemorySharedShoppingListRepository : ISharedShoppingListRepository
{
    private Dictionary<Guid, SharedListDto> _cache = new Dictionary<Guid, SharedListDto>();

    public void Save(SharedListDto list)
    {
        _cache[list.Guid] = list;
    }

    public SharedListDto? Get(Guid listToShareId)
    {
        return _cache.TryGetValue(listToShareId, out var value) ? value : null;
    }

    public List<SharedListDto> GetAllWithSourceAndCreator(Guid listId, Guid userId)
    {
        return _cache.Values.Where(list => list.CreatorId == userId && list.SourceId == listId).ToList();
    }
}