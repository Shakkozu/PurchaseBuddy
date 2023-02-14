using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductsManagementService
{
	private readonly IProductsRepository userProductsManagementRepository;

	public UserProductsManagementService(IProductsRepository userProductsManagementRepositoryy)
	{
		this.userProductsManagementRepository = userProductsManagementRepositoryy;
	}

	public IProduct DefineNewUserProduct(UserProduct product)
	{
		return userProductsManagementRepository.Save(product);
	}
	public List<IProduct> GetUserProducts(Guid userID)
	{
		return userProductsManagementRepository.GetUserProducts(userID);
	}
}
