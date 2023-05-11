using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

namespace PurchaseBuddy.src.catalogue.App;
public interface IUserProductsManagementService
{
	void AssignProductToCategory(Guid userGuid, Guid productId, Guid? categoryId);
	IProduct DefineNewUserProduct(UserProduct product);
	IProduct DefineNewUserProduct(UserProductDto productDto, Guid userId);
	List<UserProductDto> GetUserProducts(GetUserProductsQuery query);
	List<UserProductDto> GetUserProductsInCategory(GetUserProductsInCategoryQuery query);
	void Modify(Guid productId, UserProductDto request, Guid userGuid);
	void ReassignProductsToNewCategory(Guid userId, Guid categoryId, Guid value);
	void RemoveProductsFromCategory(Guid userId, Guid categoryId);
}