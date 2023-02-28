using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
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
			:UserProductCategory.CreateNewWithParent(request.Name, userID, parent, request.Description);
		productCategoriesRepository.Save(upc);

		return upc.Guid;
	}

	public List<IProductCategory> GetUserProductCategories(Guid userId)
	{
		return productCategoriesRepository.FindAll(userId);
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
}

public class CreateUserCategoryRequest
{
	public CreateUserCategoryRequest()
	{
	}

	public CreateUserCategoryRequest(string name, string? desc, Guid? parentId)
	{
		Name = name;
		Description = desc;
		ParentId = parentId;
	}
	public string Name { get; set; }
	public string? Description { get; set; }
	public Guid? ParentId { get; set; }
}

public class ProductCategoryDto
{
	public string Name { get; set; }
	public Guid Guid { get; set; }

	public ProductCategoryDto()
	{

	}
	public ProductCategoryDto(IProductCategory category)
	{
		Name = category.Name;
		Guid = category.Guid;
	}
}
