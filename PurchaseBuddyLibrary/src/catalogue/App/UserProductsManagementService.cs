using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductsManagementService
{
	private readonly IProductsRepository userProductsManagementRepository;
	private readonly UserProductCategoriesManagementService userProductCategoriesManagementService;

	public UserProductsManagementService(IProductsRepository userProductsManagementRepositoryy,
		UserProductCategoriesManagementService userProductCategoriesManagementService)
	{
		this.userProductsManagementRepository = userProductsManagementRepositoryy;
		this.userProductCategoriesManagementService = userProductCategoriesManagementService;
	}

	public IProduct DefineNewUserProduct(UserProduct product)
	{
		return userProductsManagementRepository.Save(product);
	}
	public IProduct DefineNewUserProduct(UserProductDto productDto, Guid userId)
	{
		var product = UserProduct.Create(productDto.Name, userId);
		DefineNewUserProduct(product);
		if (productDto.CategoryId != null)
			userProductCategoriesManagementService.AssignUserProductToCategory(userId, product.Guid, productDto.CategoryId.Value);

		return userProductsManagementRepository.Save(product);
	}
	public List<IProduct> GetUserProducts(Guid userID)
	{
		return userProductsManagementRepository.GetUserProducts(userID);
	}
}

public class UserProductDto
{
	public string Name { get; set; }
	public Guid? CategoryId { get; set; }
}