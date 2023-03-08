namespace PurchaseBuddy.src.catalogue.Persistance;

public class SharedProductCustomization
{
	public Guid ProductGuid { get; }
	public Guid? CategoryId { get; private set; }
	public Guid UserID { get; }
	public string Name { get; }
	public SharedProductCustomization(Guid productGuid, Guid userID, string name, Guid? categoryId = null)
	{
		ProductGuid = productGuid;
		UserID = userID;
		Name = name;
		CategoryId = categoryId;
	}
}