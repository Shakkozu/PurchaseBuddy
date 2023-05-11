namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre;

public class ProductCategoryDao
{
    public int Id { get; set; }
    public string Guid { get; set; }
    public string UserGuid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Hierarchy { get; set; }
}
