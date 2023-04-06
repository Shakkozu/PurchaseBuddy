using PurchaseBuddyLibrary.src.stores.app;

namespace PurchaseBuddy.src.catalogue.App;

public class CategoryFacade
{
	private readonly IUserProductCategoriesManagementService categoriesManagementService;
	private readonly UserProductsManagementService productsManagementService;
	private readonly IShopCategoryListManagementService categoryListManagementService;

	public CategoryFacade(IUserProductCategoriesManagementService categoriesManagementService,
		UserProductsManagementService productsManagementService,
		IShopCategoryListManagementService categoryListManagementService)
    {
		this.categoriesManagementService = categoriesManagementService;
		this.productsManagementService = productsManagementService;
		this.categoryListManagementService = categoryListManagementService;
	}
    public void RemoveCategoryAndReassignProducts(Guid userId, Guid categoryId, Guid? newCategory = null)
	{
		if (!newCategory.HasValue)
			productsManagementService.RemoveProductsFromCategory(userId, categoryId);
		else
			productsManagementService.ReassignProductsToNewCategory(userId, categoryId, newCategory.Value);

		categoryListManagementService.RemoveCategoryFromAllMaps(userId, categoryId);
		categoriesManagementService.DeleteCategory(userId, categoryId);
	}
}
