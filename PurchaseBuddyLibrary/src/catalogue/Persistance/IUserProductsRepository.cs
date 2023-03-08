using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.Persistance;

public interface IProductsRepository
{
	List<IProduct> GetUserProducts(Guid userID);
	List<IProduct> GetSharedProducts();
	IProduct Save(IProduct product);
	IProduct? GetProduct(Guid productGuid);
	void SaveSharedProductCustomization(SharedProductCustomization customization);
}
