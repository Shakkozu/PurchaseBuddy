using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.catalogue.Persistance;

public class InMemoryUserProductCategoriesRepository : IUserProductCategoriesRepository
{
	private readonly Dictionary<Guid, List<UserProductCategory>> userProductCategories = new();
	public List<UserProductCategory> FindAll(Guid userId)
	{
		return userProductCategories[userId];
	}

	public UserProductCategory? FindById(Guid userId, Guid categoryGuid)
	{
		if (userProductCategories.ContainsKey(userId))
		{
			return userProductCategories[userId].FirstOrDefault(category => category.Guid == categoryGuid);
		}

		return null;
	}

	public UserProductCategory Save(UserProductCategory userProductCategory)
	{
		if (userProductCategories.ContainsKey(userProductCategory.UserId))
		{
			userProductCategories[userProductCategory.UserId].Add(userProductCategory);
			return userProductCategory;
		}

		userProductCategories[userProductCategory.UserId] = new List<UserProductCategory> { userProductCategory };
		return userProductCategory;
	}
}
