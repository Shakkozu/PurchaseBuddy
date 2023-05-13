using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using System.Runtime.CompilerServices;

namespace PurchaseBuddy.src.stores.domain;

public record UserShopConfiguration
{
	internal static UserShopConfiguration CreateNew(Guid shopId)
	{
		return new UserShopConfiguration(shopId, new List<UserShopConfigurationEntry>());
	}

	internal static UserShopConfiguration CreateNew(Guid shopId, IList<IProductCategory>? categories)
	{
		if (categories == null)
			return UserShopConfiguration.CreateNew(shopId);

		var categoriesGuids = categories.Select(c => c.Guid).ToList();
		return CreateNew(shopId, categoriesGuids);
	}

	private static UserShopConfiguration CreateNew(Guid shopId, List<Guid> categoriesGuids)
	{
		var index = 1;
		var configurationEntries = categoriesGuids
			.Select(categoryGuid => new UserShopConfigurationEntry(index++, categoryGuid))
			.ToList();

		return new UserShopConfiguration(shopId, configurationEntries);
	}

	internal UserShopConfiguration Remove(IProductCategory category)
	{
		var entries = ConfigurationEntries
			.Where(c => c.CategoryGuid != category.Guid)
			.Select(c => c.CategoryGuid)
			.ToList();

		return CreateNew(ShopId, entries);
	}

	internal static UserShopConfiguration LoadFrom(Guid shopId, ShopDao.ShopConfigurationDao configuration)
	{
		if (configuration == null)
			return CreateNew(shopId);

		var entries = configuration.Entries
			.Select(entryDao => new UserShopConfigurationEntry(entryDao.Index, Guid.Parse(entryDao.CategoryGuid)))
			.ToList();
		return new UserShopConfiguration(shopId, entries);
	}

	private UserShopConfiguration(Guid shopId, List<UserShopConfigurationEntry> configurationEntries)
	{
		ShopId = shopId;
		ConfigurationEntries = configurationEntries;
	}
	public List<UserShopConfigurationEntry> ConfigurationEntries { get; } = new List<UserShopConfigurationEntry>();
	public Guid ShopId { get; }
}
