using Newtonsoft.Json;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.CloningListsToOtherUsers;

internal class SharedShoppingListDao
{
	public SharedShoppingListDao()
	{

	}
	public SharedShoppingListDao(SharedListDto list)
	{
		Guid = list.Guid.ToDatabaseStringFormat();
		CreatorGuid = list.CreatorId.ToDatabaseStringFormat();
		SourceListGuid = list.SourceId.ToDatabaseStringFormat();
		CreatedAt = list.CreatedAt;
		var sharedShoppingListItemDaos = list.Items
			.Select(item => new SharedShoppingListItemDao { CategoryName = item.categoryName, ProductName = item.productName })
			.ToList();
		Items = JsonConvert.SerializeObject(sharedShoppingListItemDaos);
	}


	public int Id { get; set; }
	public string Guid { get; set; }
	public string CreatorGuid { get; set; }
	public string SourceListGuid { get; set; }
	public DateTime CreatedAt { get; set; }
	public string Items { get; set; }

	internal List<SharedShoppingListItemDao> GetShoppingListEntries()
	{
		if (string.IsNullOrEmpty(Items))
			return new();

		return JsonConvert.DeserializeObject<List<SharedShoppingListItemDao>>(Items);
	}

	internal class SharedShoppingListItemDao
	{
		public string ProductName { get; set; }
		public string? CategoryName { get; set; }
	}
}
