namespace PurchaseBuddyLibrary.src.utils;
public static class GuidExtensions
{
	public static string ToDatabaseStringFormat(this Guid guid)
	{
		return guid.ToString("D");
	}
}
