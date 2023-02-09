namespace PurchaseBuddy.src.stores.domain;

public class UserShopDescription
{
	public static UserShopDescription CreateNew(string name, string? description = null, Address? address = null)
	{
		return new UserShopDescription(name, description, address);
	}
	private UserShopDescription(string name, string? description, Address? address)
	{
		Name = name;
		Description = description;
		Address = address;
	}
	public string Name { get; }
	public string? Description { get; }
	public Address? Address { get; }
}
