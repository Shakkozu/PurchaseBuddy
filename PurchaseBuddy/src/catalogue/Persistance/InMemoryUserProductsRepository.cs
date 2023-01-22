using PurchaseBuddy.src.catalogue.Model;

namespace PurchaseBuddy.src.catalogue.Persistance;

public class InMemoryUserProductsRepository : IUserProductsRepository
{
	private readonly Dictionary<Guid, List<UserProduct>> usersProducts = new();

	public UserProduct? GetUserProduct(Guid userId, Guid productGuid)
	{
		if (!usersProducts.ContainsKey(userId))
			return null;

		return usersProducts[userId].FirstOrDefault(product => product.Guid == productGuid);
	}

	public List<UserProduct> GetUserProducts(Guid userID)
	{
		if (usersProducts.ContainsKey(userID))
			return usersProducts[userID];

		return new List<UserProduct>();
	}

	public UserProduct Save(UserProduct product)
	{
		if (usersProducts.ContainsKey(product.UserID))
			usersProducts[product.UserID].Add(product);
		else
			usersProducts[product.UserID] = new List<UserProduct> { product };

		return product;
	}
}