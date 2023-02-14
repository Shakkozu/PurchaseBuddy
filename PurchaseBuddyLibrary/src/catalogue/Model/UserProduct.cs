namespace PurchaseBuddy.src.catalogue.Model;

//public interface IProduct
//{
//	public int Id { get; set; }
//	public Guid Guid { get; set; }
//}
public class SharedProduct
{
	public int Id { get; private set; }
	public Guid Guid { get; private set; }
	public string Name { get; private set; }
	
	public static SharedProduct CreateNew(string name)
	{
		return new SharedProduct(name, Guid.NewGuid());
	}
	private SharedProduct(string name, Guid guid)
	{
		Name = name;
		Guid = guid;
	}
}
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
