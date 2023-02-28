namespace PurchaseBuddy.src.stores.domain;
public class UserShop
{
	public Guid Guid { get; set; }
	public Guid UserId { get; set; }
	public List<UserShopConfigurationEntry> ConfigurationEntries => configuration.ConfigurationEntries;

	private UserShopConfiguration configuration;
	public UserShopDescription Description { get; private set; }

	public static UserShop CreateNew(Guid userId, UserShopDescription userShopDescription)
	{
		var guid = Guid.NewGuid();
		var userShopConfiguration = UserShopConfiguration.CreateNew(guid);
		return new UserShop(guid, userId, userShopDescription, userShopConfiguration);
	}

	public void ChangeDescriptionTo(UserShopDescription userShopDescription)
	{
		Description = userShopDescription;
	}

	public void ModifyShopConfiguration(List<Guid> orderedShopCategories)
	{
		configuration = UserShopConfiguration.CreateNew(Guid, orderedShopCategories);
	}

	private UserShop(Guid guid, Guid userId, UserShopDescription userShopDescription, UserShopConfiguration configuration)
	{
		Guid = guid;
		UserId = userId;
		Description = userShopDescription;
		this.configuration = configuration;
	}
}
