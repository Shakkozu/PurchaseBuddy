using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddyLibrary.src.stores.contract;
public class UserShopDto
{
	public UserShopDto()
	{

	}
	public static UserShopDto FromModel(UserShop? userShop)
	{
		if (userShop == null)
			return null;

		return new UserShopDto(userShop.Guid,
						 userShop.Description.Name,
						 userShop.Description.Description,
						 userShop.Description.Address,
						 userShop.ConfigurationEntries);
	}

	private UserShopDto(Guid guid, string name, string? description, Address? address, List<UserShopConfigurationEntry> entries)
	{
		Guid = guid;
		Name = name;
		Description = description;
		if (address == null)
			return;

		Address = new AddressDto
		{
			City = address.City,
			LocalNumber = address.LocalNumber,
			Street = address.Street
		};
		CategoriesMap = entries.Select(entry => entry.CategoryGuid).ToList();
	}

	public Guid? Guid { get; set; }
	public string Name { get; set; }
	public string? Description { get; set; }
	public AddressDto Address { get; set; }
	public List<Guid> CategoriesMap { get; set; } = new();
}
