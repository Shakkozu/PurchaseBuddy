using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.domain;
using System.Linq;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;
public class InMemoryShoppingListInvitationsRepository : IShoppingListInvitationsRepository
{
    private Dictionary<Guid, ShoppingInvitationsList> cache = new();
    public ShoppingInvitationsList? Get(Guid shoppingListGuid)
    {
        if (!cache.ContainsKey(shoppingListGuid))
            return null;

        return cache[shoppingListGuid];
    }

    public IEnumerable<ShoppingInvitationsList> GetAllWhereUserIsOnInvitationsList(Guid userGuid)
    {
        return cache.Values.Where(x => x.UsersInvitedToModify.Any(invite => invite.UserId == userGuid) && x.IsActive).ToList();
    }

    public void Save(ShoppingInvitationsList shoppingList)
    {
        cache[shoppingList.ListId] = shoppingList;
    }

    public void Update(ShoppingInvitationsList shoppingList)
    {
        cache[shoppingList.ListId] = shoppingList;
    }
}
