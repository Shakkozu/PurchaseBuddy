namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;
public class UserProduct : IProduct
{
    public int Id { get; }
    public Guid Guid { get; }
    public Guid UserID { get; }
    public string Name { get; }
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
