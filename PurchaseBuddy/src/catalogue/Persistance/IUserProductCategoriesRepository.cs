using PurchaseBuddy.src.catalogue.Model;

namespace PurchaseBuddy.src.catalogue.Persistance;

public interface IUserProductCategoriesRepository
{
	UserProductCategory Save(UserProductCategory userProductCategory);
	List<UserProductCategory> FindAll(Guid userId);
	UserProductCategory? FindById(Guid userId, Guid categoryGuid);
}
