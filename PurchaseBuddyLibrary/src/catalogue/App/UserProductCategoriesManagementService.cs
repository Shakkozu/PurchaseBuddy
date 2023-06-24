using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProductCategories;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductCategoriesManagementService : IUserProductCategoriesManagementService
{
	private readonly IUserProductCategoriesRepository productCategoriesRepository;
	private readonly IProductsRepository userProductsRepository;
	private readonly GetUserProductCategoriesQueryHandler getUserProductCategoriesQueryHandler;

	public UserProductCategoriesManagementService(
		IUserProductCategoriesRepository productCategoriesRepository,
		IProductsRepository userProductsRepository)
	{
		this.productCategoriesRepository = productCategoriesRepository;
		this.userProductsRepository = userProductsRepository;
		getUserProductCategoriesQueryHandler = new GetUserProductCategoriesQueryHandler(productCategoriesRepository);
	}

	public GetUserProductCategoriesResponse GetUserProductCategories(Guid userId)
	{
		return getUserProductCategoriesQueryHandler.Handle(userId);
	}
	public Guid AddNewProductCategory(Guid userID, CreateUserCategoryRequest request)
	{
		IProductCategory? parent = null;
		if (request.ParentId.HasValue)
			parent = productCategoriesRepository.FindById(userID, request.ParentId.Value);

		var upc = parent == null
			? UserProductCategory.CreateNew(request.Name, userID, request.Description)
			: UserProductCategory.CreateNewWithParent(request.Name, userID, parent, request.Description);

		if (parent != null)
			productCategoriesRepository.Update(parent);

		productCategoriesRepository.Save(upc);
		return upc.Guid;
	}

	public List<IProductCategory> GetCategories(Guid userId)
	{
		return productCategoriesRepository.FindAll(userId)
			.Where(cat => cat.IsRoot)
			.ToList();
	}

	public IEnumerable<IProductCategory> GetCategoriesAsFlatList(Guid userId)
	{
		var result = new List<IProductCategory>();
		var categories = productCategoriesRepository.FindAll(userId);
		if (!categories.Any())
			return result;
		// Use a hash set to keep track of visited categories
		var visited = new HashSet<Guid>();
		var rootCategories = categories.Where(c => c.IsRoot);

		// Recursively get all categories
		foreach (var category in rootCategories)
			result.AddRange(GetAllCategoriesRecursive(category, visited));

		return result;
	}

	private static IEnumerable<IProductCategory> GetAllCategoriesRecursive(IProductCategory category, HashSet<Guid> visited)
	{
		if (visited.Contains(category.Guid))
			return Enumerable.Empty<IProductCategory>();

		visited.Add(category.Guid);
		var result = new List<IProductCategory> { category };

		// Recursively get all child categories
		foreach (var child in category.Children)
			result.AddRange(GetAllCategoriesRecursive(child, visited));

		return result;
	}

	public void DeleteCategory(Guid userId, Guid categoryId)
	{
		var category = productCategoriesRepository.FindById(userId, categoryId);
		if (category is null)
			throw new ResourceNotFoundException($"user product category with id {categoryId} not found for user: {userId}");
		if (category is SharedProductCategory)
			throw new InvalidOperationException();

		if (category.ParentId.HasValue)
		{
			var parentCategory = productCategoriesRepository.FindById(userId, category.ParentId.Value);
			if (parentCategory is null)
				throw new ResourceNotFoundException($"user product category with id {category.ParentId.Value} not found for user: {userId}");

			foreach (var child in category.Children.ToList())
			{
				category.RemoveChild(child);
				parentCategory.AddChild(child);
				productCategoriesRepository.Update(child);
			}
			parentCategory.RemoveChild(category);
			productCategoriesRepository.Remove(category);
			return;
		}

		foreach (var child in category.Children.ToList())
		{
			child.RemoveParent();
			productCategoriesRepository.Update(child);
		}
		productCategoriesRepository.Remove(category);
	}

	public void AssignProductToCategory(Guid userId, Guid productGuid, Guid categoryGuid)
	{
		var userProduct = userProductsRepository.GetProduct(productGuid, userId);
		if (userProduct == null)
			throw new ResourceNotFoundException($"user product with categoryId {productGuid} not found for user: {userId}");

		var category = productCategoriesRepository.FindById(userId, categoryGuid);
		if (category is null)
			throw new ResourceNotFoundException($"user product category with id {categoryGuid} not found for user: {userId}");

		category.AddProduct(userProduct);
		productCategoriesRepository.Save(category);
	}

	public void ReassignCategory(Guid userId, Guid productCategoryGuid, Guid newParentCategoryGuid)
	{
		var categories = productCategoriesRepository.FindAll(userId);
		var category = productCategoriesRepository.FindById(userId, productCategoryGuid);
		if (category is null)
			throw new ResourceNotFoundException($"user product category with id {productCategoryGuid} not found for user: {userId}");

		var newParentCategory = categories.FirstOrDefault(c => c.Guid == newParentCategoryGuid);
		if (newParentCategory is null)
			throw new ResourceNotFoundException($"user product category with id {newParentCategoryGuid} not found for user: {userId}");

		if (category.ParentId.HasValue)
		{
			var currentParentCategory = categories.FirstOrDefault(c => c.Guid == category.ParentId.Value);
			if (currentParentCategory is null)
				throw new ResourceNotFoundException($"user product category with id {category.ParentId.Value} not found for user: {userId}");

			currentParentCategory.RemoveChild(category);
			productCategoriesRepository.Update(currentParentCategory);
		}
		newParentCategory.AddChild(category);

		productCategoriesRepository.Update(newParentCategory);
		productCategoriesRepository.Update(category);
	}

	public IProductCategory? GetUserProductCategory(Guid userId, Guid categoryId)
	{
		return productCategoriesRepository.FindById(userId, categoryId);
	}
}


