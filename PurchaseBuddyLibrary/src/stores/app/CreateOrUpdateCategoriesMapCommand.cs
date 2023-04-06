namespace PurchaseBuddyLibrary.src.stores.app;

public class CreateOrUpdateCategoriesMapCommand
{
    public Guid UserId { get; set; }
    public Guid ShopId { get; set; }
	public List<Guid> CategoriesMap { get; set; } = new List<Guid> { };
}
