using PurchaseBuddy.src.catalogue.Model;

namespace PurchaseBuddy.src.catalogue.Persistance;

public interface IUserProductsRepository
{
	List<UserProduct> GetUserProducts(Guid userID);
	UserProduct Save(UserProduct product);
	UserProduct? GetUserProduct(Guid userId, Guid productGuid);
}
