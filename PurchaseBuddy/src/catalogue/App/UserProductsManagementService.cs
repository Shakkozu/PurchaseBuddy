using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductsManagementService
{
	private readonly IUserProductsRepository userProductsManagementRepository;
	private readonly IUserProductCategoriesRepository userProductCategoriesRepository;

	public UserProductsManagementService(IUserProductsRepository userProductsManagementRepository, IUserProductCategoriesRepository userProductCategoriesRepository)
	{
		this.userProductsManagementRepository = userProductsManagementRepository;
		this.userProductCategoriesRepository = userProductCategoriesRepository;
	}


	public UserProduct DefineNewUserProduct(UserProduct product)
	{
		return userProductsManagementRepository.Save(product);
	}
	public List<UserProduct> GetUserProducts(Guid userID)
	{
		return userProductsManagementRepository.GetUserProducts(userID);
	}
	public UserProduct GetUserProduct(Guid userID, Guid productId)
	{
		var product = userProductsManagementRepository.GetUserProduct(userID, productId);
		if (product is null)
			throw new ResourceNotFoundException();

		return product;
	}
}
