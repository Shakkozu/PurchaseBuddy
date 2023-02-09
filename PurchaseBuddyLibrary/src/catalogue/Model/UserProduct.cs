namespace PurchaseBuddy.src.catalogue.Model;

public class UserProduct
{
	public int Id { get; set; }
	public Guid Guid { get; set; }
	public Guid UserID { get; set; }
	public string Name { get; set; }
	public static UserProduct Create(string name, Guid userId)
	{
		return new UserProduct(null, userId, name, Guid.NewGuid());
	}
	private UserProduct(int? id, Guid userID, string name, Guid guid)
	{
		if (id.HasValue)
			Id = id.Value;

		UserID = userID;
		Name = name;
		Guid = guid;
	}
}
