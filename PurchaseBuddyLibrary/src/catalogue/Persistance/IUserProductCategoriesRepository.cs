using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.catalogue.Persistance;

public interface IUserProductCategoriesRepository
{
	IProductCategory Save(IProductCategory productCategory);
	List<IProductCategory> FindAll(Guid userId);
	IProductCategory? FindById(Guid userId, Guid categoryGuid);
	void Remove(IProductCategory category);
}
