using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.Persistance;

public class InMemoryProductsRepository : IProductsRepository
{
	private readonly Dictionary<Guid, IProduct> products = new();
	public IProduct? GetProduct(Guid productGuid)
	{
		if (products.ContainsKey(productGuid))
			return products[productGuid];

		return null;
	}

	public List<IProduct> GetUserProducts(Guid userID)
	{
		return products.Where(product => {
			if (product.Value is UserProduct userProduct)
				return userProduct.UserID == userID;
			return true;
		}).Select(product => product.Value).ToList();
	}

	public IProduct Save(IProduct product)
	{
		if (products.ContainsKey(product.Guid))
			products[product.Guid] = product;
		else
			products.Add(product.Guid, product);

		return product;
	}
}
