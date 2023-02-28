namespace PurchaseBuddy.src.catalogue.Model;

public class SharedProduct
{
    public string Name { get; }
    public Guid Guid { get; }

    public static SharedProduct Create(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");

        return new SharedProduct(name, Guid.NewGuid());
    }
    private SharedProduct(string name, Guid guid)
    {
        Name = name;
        Guid = guid;
    }
}
