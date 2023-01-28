namespace PurchaseBuddy.src.stores.domain;

public class UserShopConfigurationEntry
{
	public UserShopConfigurationEntry(int index, Guid categoryGuid)
	{
		Index = index;
		CategoryGuid = categoryGuid;
	}
	public int Index { get; }
	public Guid CategoryGuid { get; }
}
