namespace PurchaseBuddy.src.catalogue.App;

public class ProductsFacade
{
	private readonly UserProductCategoriesManagementService categoriesManagementService;
	private readonly UserProductsManagementService productsManagementService;

	public ProductsFacade(UserProductCategoriesManagementService categoriesManagementService,
		UserProductsManagementService productsManagementService)
    {
		this.categoriesManagementService = categoriesManagementService;
		this.productsManagementService = productsManagementService;
	}
    public void RemoveCategoryAndReassignProducts(Guid userId, Guid categoryId, Guid? newCategory = null)
	{
		if (!newCategory.HasValue)
			productsManagementService.RemoveProductsFromCategory(userId, categoryId);
		else
			productsManagementService.ReassignProductsToNewCategory(userId, categoryId, newCategory.Value);

		categoriesManagementService.DeleteCategory(userId, categoryId);
	}
}
