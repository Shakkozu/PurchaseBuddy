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
}