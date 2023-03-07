using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductCategoriesManagementService
{
	private readonly IUserProductCategoriesRepository productCategoriesRepository;
	private readonly IProductsRepository userProductsRepository;

	public UserProductCategoriesManagementService(IUserProductCategoriesRepository productCategoriesRepository, IProductsRepository userProductsRepository)
	{
		this.productCategoriesRepository = productCategoriesRepository;
		this.userProductsRepository = userProductsRepository;
	}
	public Guid AddNewProductCategory(Guid userID, CreateUserCategoryRequest request)
	{
		IProductCategory? parent = null;
		if (request.ParentId.HasValue)
			parent = productCategoriesRepository.FindById(userID, request.ParentId.Value);

		var upc = parent == null
			? UserProductCategory.CreateNew(request.Name, userID, request.Description)
			: UserProductCategory.CreateNewWithParent(request.Name, userID, parent, request.Description);

		if(parent != null)
			productCategoriesRepository.Save(parent);

		productCategoriesRepository.Save(upc);
		return upc.Guid;
	}

	public List<IProductCategory> GetUserProductCategories(Guid userId)
	{
		return productCategoriesRepository.FindAll(userId);
	}

	public List<ProductCategoryDto> GetProductCategories2(Guid userId)
	{
		return productCategoriesRepository.FindAll(userId)
			.Where(cat => cat.IsRoot)
			.Select(category => new ProductCategoryDto(category))
			.ToList();
	}

	public void AssignUserProductToCategory(Guid userId, Guid productGuid, Guid categoryGuid)
	{
		var userProduct = userProductsRepository.GetProduct(productGuid);
		if (userProduct == null)
			throw new ResourceNotFoundException($"user product with categoryId {productGuid} not found for user: {userId}");

		var category = productCategoriesRepository.FindById(userId, categoryGuid);
		if (category is null)
			throw new ResourceNotFoundException($"user product category with id {categoryGuid} not found for user: {userId}");

		category.AddProduct(userProduct);
		productCategoriesRepository.Save(category);
	}

	public void ReassignUserProductCategory(Guid userId, Guid productCategoryGuid, Guid newParentCategoryGuid)
	{
		var category = productCategoriesRepository.FindById(userId, productCategoryGuid);
		if(category is null)
			throw new ResourceNotFoundException($"user product category with id {productCategoryGuid} not found for user: {userId}");

		var newParentCategory = productCategoriesRepository.FindById(userId, newParentCategoryGuid);
		if (newParentCategory is null)
			throw new ResourceNotFoundException($"user product category with id {newParentCategoryGuid} not found for user: {userId}");

		
		if(category.ParentId.HasValue)
		{
			var currentParentCategory = productCategoriesRepository.FindById(userId, category.ParentId.Value);
			if (currentParentCategory is null)
				throw new ResourceNotFoundException($"user product category with id {category.ParentId.Value} not found for user: {userId}");

			currentParentCategory.RemoveChild(category);
			productCategoriesRepository.Save(currentParentCategory);
		}
		newParentCategory.AddChild(category);

		productCategoriesRepository.Save(newParentCategory);
		productCategoriesRepository.Save(category);
	}
}


