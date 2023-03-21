using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.catalogue.Persistance;

public class InMemoryUserProductCategoriesRepository : IUserProductCategoriesRepository
{
	private readonly Dictionary<Guid, IProductCategory> productCategories = new();
	public List<IProductCategory> FindAll(Guid userId)
	{
		return productCategories.Values.Where(category =>
		{
			if (category is UserProductCategory)
				return (category as UserProductCategory).UserId == userId;

			return true;
		}).ToList();
	}

	public IProductCategory? FindById(Guid userId, Guid categoryGuid)
	{
		if (productCategories.ContainsKey(categoryGuid))
		{
			var category = productCategories[categoryGuid];
			var userProductCategory = category as UserProductCategory;
			if (userProductCategory is not null && userProductCategory.UserId != userId)
				return null;

			return category;
		}

		return null;
	}

	public void Remove(IProductCategory category)
	{
		if(productCategories.ContainsKey(category.Guid))
			productCategories.Remove(category.Guid);
	}

	public IProductCategory Save(IProductCategory productCategory)
	{
		productCategories[productCategory.Guid] = productCategory;
		return productCategory;
	}
}
