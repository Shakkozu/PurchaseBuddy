using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.catalogue.App;
public interface IUserProductCategoriesManagementService
{
	Guid AddNewProductCategory(Guid userID, CreateUserCategoryRequest request);
	void AssignProductToCategory(Guid userId, Guid productGuid, Guid categoryGuid);
	void DeleteCategory(Guid userId, Guid categoryId);
	List<IProductCategory> GetCategories(Guid userId);
	IProductCategory? GetUserProductCategory(Guid userId, Guid value);
	void ReassignCategory(Guid userId, Guid productCategoryGuid, Guid newParentCategoryGuid);
}