using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.Persistance;

public interface IProductsRepository
{
	List<IProduct> GetUserProducts(Guid userID);
	IProduct Save(IProduct product);
	IProduct? GetProduct(Guid productGuid, Guid userId);
	void SaveSharedProductCustomization(SharedProductCustomization customization);
	void Update(IProduct product, Guid UserId);
}
