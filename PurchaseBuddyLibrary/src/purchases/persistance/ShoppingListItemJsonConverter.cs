using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddyLibrary.purchases.persistance;

internal class ShoppingListItemJsonConverter
{
	internal List<ShoppingListItemDao> Convert(string json)
	{
		var settings = new JsonSerializerSettings();
		settings.Converters.Add(new ShoppingListItemConverter());
		return JsonConvert.DeserializeObject<List<ShoppingListItemDao>>(json, settings);
	}
}

internal class ShoppingListItemConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType.FullName.Contains("ShoppingListItem");
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JArray array = JArray.Load(reader);
		List<ShoppingListItemDao> items = new List<ShoppingListItemDao>();
		foreach (JObject itemObject in array.Children<JObject>())
		{
			string itemType = itemObject["Type"].Value<string>();
			ShoppingListItemDao item;
			switch (itemType)
			{
				case ShoppingListItemTypes.Imported:
					item = new ImportedShoppingListItemDao();
					break;
				case ShoppingListItemTypes.UserDefined:
					item = new UserShoppingListItemDao();
					break;
				default:
					throw new NotSupportedException($"Unsupported item type: {itemType}");
			}

			// Populate the properties of the chosen derived type
			serializer.Populate(itemObject.CreateReader(), item);
			items.Add(item);
		}

		return items;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		// required but not used
	}
}
