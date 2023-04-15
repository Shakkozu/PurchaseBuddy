using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.src.stores.domain;
public class UserShop
{
	public Guid Guid { get; set; }
	public Guid UserId { get; set; }
	public List<UserShopConfigurationEntry> ConfigurationEntries => configuration.ConfigurationEntries;

	private UserShopConfiguration configuration;
	public UserShopDescription Description { get; private set; }

	public static UserShop CreateNew(Guid userId, UserShopDescription userShopDescription, List<IProductCategory>? orderedShopCategories = null)
	{
		var guid = Guid.NewGuid();
		var userShopConfiguration = UserShopConfiguration.CreateNew(guid, orderedShopCategories);

		return new UserShop(guid, userId, userShopDescription, userShopConfiguration);
	}

	public void ChangeDescriptionTo(UserShopDescription userShopDescription)
	{
		Description = userShopDescription;
	}

	public void ModifyShopConfiguration(IList<IProductCategory> orderedShopCategories)
	{
		configuration = UserShopConfiguration.CreateNew(Guid, orderedShopCategories);
	}

	internal void RemoveCategoryFromConfiguration(IProductCategory category)
	{
		configuration = configuration.Remove(category);
	}

	private UserShop(Guid guid, Guid userId, UserShopDescription userShopDescription, UserShopConfiguration configuration)
	{
		Guid = guid;
		UserId = userId;
		Description = userShopDescription;
		this.configuration = configuration;
	}
}
