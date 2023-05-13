using PurchaseBuddy.src.stores.persistance;
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

	internal static UserShop LoadFrom(ShopDao dao)
	{
		var shopGuid = Guid.Parse(dao.Guid);
		return new UserShop(
			shopGuid,
			Guid.Parse(dao.UserGuid),
			UserShopDescription.CreateNew(dao.Name, dao.Description,
				new Address(dao.Street, dao.City, dao.LocalNumber)),
			UserShopConfiguration.LoadFrom(shopGuid, dao.GetConfiguration())
			);
	}

	private UserShop(Guid guid, Guid userId, UserShopDescription userShopDescription, UserShopConfiguration configuration)
	{
		Guid = guid;
		UserId = userId;
		Description = userShopDescription;
		this.configuration = configuration;
	}
}
