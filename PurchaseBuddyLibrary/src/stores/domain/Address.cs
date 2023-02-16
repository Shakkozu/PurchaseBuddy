using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddy.src.stores.domain;

public class Address
{
	public Address(string street, string city, string localNumber)
	{
		Street = street;
		City = city;
		LocalNumber = localNumber;
	}

	public string Street { get; }
	public string City { get; }
	public string LocalNumber { get; }

	public static Address From(AddressDto address)
	{
		return new Address(address.Street, address.City, address.LocalNumber);
	}
}