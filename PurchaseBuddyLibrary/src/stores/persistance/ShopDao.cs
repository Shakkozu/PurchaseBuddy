using Newtonsoft.Json;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.src.stores.persistance;

public class ShopDao
{
    public ShopDao()
    {
        
    }
    public ShopDao(UserShop userShop)
    {
		Guid = userShop.Guid.ToDatabaseStringFormat();
		Name = userShop.Description.Name;
		var address = userShop.Description.Address;
		Description = userShop.Description.Description;
		Street = address?.Street;
		City = address?.City;
		LocalNumber = address?.LocalNumber;
		UserGuid = userShop.UserId.ToDatabaseStringFormat();
		configuration = new ShopConfigurationDao(userShop.ConfigurationEntries);
		ConfigurationString = JsonConvert.SerializeObject(configuration);
    }

	public class ShopConfigurationDao
	{
        public ShopConfigurationDao()
        {
            
        }
        public ShopConfigurationDao(List<UserShopConfigurationEntry> configurationEntries)
        {
			foreach (var entry in configurationEntries.OrderBy(x => x.Index))
				Entries.Add(new ShopConfigurationEntryDao(entry.Index, entry.CategoryGuid));
        }

		public List<ShopConfigurationEntryDao> Entries { get; set; } = new List<ShopConfigurationEntryDao>();

		public record ShopConfigurationEntryDao
		{
			public ShopConfigurationEntryDao(int index, Guid categoryGuid)
			{
				Index = index;
				CategoryGuid = categoryGuid.ToDatabaseStringFormat();
			}
            public int Index { get; }
			public string CategoryGuid { get;}
		}
    }
	public ShopConfigurationDao GetConfiguration()
	{
		return JsonConvert.DeserializeObject<ShopConfigurationDao>(ConfigurationString);
	}

	public int Id { get; set; }
    public string Guid { get; set; }
    public string UserGuid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string LocalNumber { get; set; }
    public string ConfigurationString { get; set; }

	private ShopConfigurationDao configuration { get; set; }
}
