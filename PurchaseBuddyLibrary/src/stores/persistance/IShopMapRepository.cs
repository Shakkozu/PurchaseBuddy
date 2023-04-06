using PurchaseBuddyLibrary.src.stores.app;

namespace PurchaseBuddyLibrary.src.stores.persistance;

public interface IShopMapRepository
{
    ShopMap? Get(Guid userId, Guid shopId);
    List<ShopMap> GetAll(Guid userID);
    void Save(ShopMap shopMap);
}
