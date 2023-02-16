using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddyLibrary.src.stores.contract;
public class UserShopDto
{
	public static UserShopDto FromModel(UserShop userShop)
	{
		return new UserShopDto(userShop.Guid, userShop.Description.Name, userShop.Description.Description, userShop.Description.Address);
	}

	private UserShopDto(Guid guid, string name, string? description, Address? address)
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
	}

	public Guid Guid { get; set; }
	public string Name { get; set; }
	public string? Description { get; set; }
	public AddressDto Address { get; set; }
}