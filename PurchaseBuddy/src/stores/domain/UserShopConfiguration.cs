namespace PurchaseBuddy.src.stores.domain;

public class UserShopConfiguration
{
	internal static UserShopConfiguration CreateNew(Guid shopId)
	{
		return new UserShopConfiguration(shopId, new List<UserShopConfigurationEntry>());
	}

	internal static UserShopConfiguration CreateNew(Guid shopId, List<Guid> configurationGuids)
	{
		var index = 1;
		var configurationEntries = configurationGuids
			.Select(configurationGuid => new UserShopConfigurationEntry(index++, configurationGuid))
			.ToList();

		return new UserShopConfiguration(shopId, configurationEntries);
	}

	private UserShopConfiguration(Guid shopId, List<UserShopConfigurationEntry> configurationEntries)
	{
		ShopId = shopId;
		ConfigurationEntries = configurationEntries;
	}
	public List<UserShopConfigurationEntry> ConfigurationEntries { get; } = new List<UserShopConfigurationEntry>();
	public Guid ShopId { get; }
}
