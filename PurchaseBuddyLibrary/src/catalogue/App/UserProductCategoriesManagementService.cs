using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductCategoriesManagementService
{
	private readonly IUserProductCategoriesRepository productCategoriesRepository;
	private readonly IUserProductsRepository userProductsRepository;

	public UserProductCategoriesManagementService(IUserProductCategoriesRepository productCategoriesRepository, IUserProductsRepository userProductsRepository)
	{
		this.productCategoriesRepository = productCategoriesRepository;
		this.userProductsRepository = userProductsRepository;
	}
	public void AddNewProductCategory(UserProductCategory productCategory)
	{
		productCategoriesRepository.Save(productCategory);
	}

	public List<UserProductCategory> GetUserProductCategories(Guid userId)
	{
		return productCategoriesRepository.FindAll(userId);
	}

	public void AssignUserProductToCategory(Guid userId, Guid productGuid, Guid categoryGuid)
	{
		var userProduct = userProductsRepository.GetUserProduct(userId, productGuid);
		if (userProduct == null)
			throw new ResourceNotFoundException($"user product with categoryId {productGuid} not found for user: {userId}");

		var category = productCategoriesRepository.FindById(userId, categoryGuid);
		if (category is null)
			throw new ResourceNotFoundException($"user product category with id {categoryGuid} not found for user: {userId}");

		category.AddProduct(userProduct);
		productCategoriesRepository.Save(category);
	}
}
