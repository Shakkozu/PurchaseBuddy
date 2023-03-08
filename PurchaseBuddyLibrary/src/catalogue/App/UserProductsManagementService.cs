using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.App.Queries;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductsManagementService
{
	private readonly IProductsRepository productsRepository;
	private readonly GetUserProductsQueryHandler queryHandler;
	private readonly UserProductCategoriesManagementService userProductCategoriesManagementService;

	public UserProductsManagementService(IProductsRepository userProductsRepository,
		UserProductCategoriesManagementService userProductCategoriesManagementService)
	{
		this.productsRepository = userProductsRepository;
		this.queryHandler = new GetUserProductsQueryHandler(userProductsRepository, userProductCategoriesManagementService);
		this.userProductCategoriesManagementService = userProductCategoriesManagementService;

		AddSharedProducts();
	}

	private void AddSharedProducts()
	{
		for(int i = 0; i < 5; i++)
		{
			productsRepository.Save(SharedProduct.CreateNew("TestProduct" + i));
		}
	}
	public void AssignProductToCategory(Guid userGuid, Guid productId, Guid categoryId)
	{
		var product = productsRepository.GetProduct(productId);
		if(product == null)
			throw new ResourceNotFoundException($"Product {productId} not found for user {userGuid}");

		var category = userProductCategoriesManagementService.GetUserProductCategory(userGuid, categoryId);
		if(category is null)
			throw new ResourceNotFoundException("Category not found");

		if (product is SharedProduct)
		{
			var customization = new SharedProductCustomization(productId, userGuid, product.Name, categoryId);
			productsRepository.SaveSharedProductCustomization(customization);
		}
		else
			product.AssignProductToCategory(category);
		

		productsRepository.Save(product);
	}

	public IProduct DefineNewUserProduct(UserProduct product)
	{
		return productsRepository.Save(product);
	}
	public IProduct DefineNewUserProduct(UserProductDto productDto, Guid userId)
	{
		if(productDto.CategoryId.HasValue)
		{
			var category = userProductCategoriesManagementService.GetUserProductCategory(userId, productDto.CategoryId.Value);
			if(category is null)
				throw new ResourceNotFoundException($"product category {productDto.CategoryId.Value} not found for user {userId}");
		}

		var product = productDto.CategoryId.HasValue
			? UserProduct.Create(productDto.Name, userId, productDto.CategoryId)
			: UserProduct.Create(productDto.Name, userId);

		return productsRepository.Save(product);
	}

	public List<UserProductDto> GetUserProducts(GetUserProductsQuery query)
	{
		return queryHandler.Handle(query);
	}
}
