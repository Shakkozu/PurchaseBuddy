using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.stores.app;

public class ShopMap
{
	public ShopMap(Guid userId, Guid shopId, List<Guid> categoriesMap)
	{
		UserId = userId;
		ShopId = shopId;
		UpdateCategoriesMap(categoriesMap);
	}

    public Guid UserId { get; }
    public Guid ShopId { get; }
	public IList<Guid> Categories { get; private set; } = new List<Guid>();

	internal void UpdateCategoriesMap(List<Guid> categoriesMap)
	{
		Categories = categoriesMap.Distinct().ToList();
	}
}