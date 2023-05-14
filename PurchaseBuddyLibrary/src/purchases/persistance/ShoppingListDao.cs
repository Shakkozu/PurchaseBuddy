using Newtonsoft.Json;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.src.purchases.persistance;

public class ShoppingListDao
{
    public ShoppingListDao()
    {
        
    }
    public ShoppingListDao(ShoppingList shoppingList)
	{
		UserGuid = shoppingList.UserId.ToDatabaseStringFormat();
		ShopGuid = shoppingList.ShopId.HasValue ? shoppingList.ShopId.Value.ToDatabaseStringFormat() : null;
		Guid = shoppingList.Guid.ToDatabaseStringFormat();
		CreatedAt = shoppingList.CreatedAt;
		CompletedAt = shoppingList.CompletedAt;
		shoppingListItems = shoppingList.Items.Select(item => new ShoppingListItemDao(item)).ToList();
		ItemsString = JsonConvert.SerializeObject(shoppingListItems);
	}

	public int Id { get; set; }
	public string UserGuid { get; set; }
	public string Guid { get; set; }
	public string? ShopGuid { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? CompletedAt { get; set; }
	public string ItemsString { get; set; }
	private List<ShoppingListItemDao> shoppingListItems = new();

	public record ShoppingListItemDao
	{
        public ShoppingListItemDao()
        {
            
        }
        public ShoppingListItemDao(ShoppingListItem item)
        {
			ItemGuid = item.ProductId.ToDatabaseStringFormat();
			Quantity = item.Quantity;
			Purchased = item.Purchased;
			Unavailable = item.Unavailable;
        }
        public string ItemGuid { get; set; }
        public int Quantity { get; set; }
		public bool Purchased { get; set; }
		public bool Unavailable { get; set; }
	}

	internal List<ShoppingListItemDao> GetShoppingListEntries()
	{
		if (string.IsNullOrEmpty(ItemsString))
			return new();

		var list = JsonConvert.DeserializeObject<List<ShoppingListItemDao>>(ItemsString);
		if (list == null)
			return new();

		return list;
	}
}
