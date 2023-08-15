using Newtonsoft.Json;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.persistance;
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
		shoppingListItems = ShoppingListItemMapper.Map(shoppingList.Items).ToList();
		ItemsString = JsonConvert.SerializeObject(shoppingListItems);
		UsersAllowedToModify = string.Join(_listSeparator, shoppingList.UsersAllowedToModify.Select(x => x.ToDatabaseStringFormat()));
	}

	public int Id { get; set; }
	public string UserGuid { get; set; }
	public string Guid { get; set; }
	public string? ShopGuid { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? CompletedAt { get; set; }
	public string ItemsString { get; set; }
	public string UsersAllowedToModify { get; internal set; }

	private List<ShoppingListItemDao> shoppingListItems = new();

	internal List<ShoppingListItemDao> GetShoppingListEntries()
	{
		if (string.IsNullOrEmpty(ItemsString))
			return new();

		var converter = new ShoppingListItemJsonConverter();
		return converter.Convert(ItemsString);
	}

	internal List<Guid> GetUsersAllowedToModify()
	{
		if (string.IsNullOrEmpty(UsersAllowedToModify))
			return new List<Guid>();

		var users = UsersAllowedToModify.Split(_listSeparator, StringSplitOptions.TrimEntries);
		return users.Select(System.Guid.Parse).ToList();
	}

	private const string _listSeparator = ";";
}
