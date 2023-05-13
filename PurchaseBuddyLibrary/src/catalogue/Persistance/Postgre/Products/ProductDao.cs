using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;

public class ProductDao
{
    private string name;
    private string? categoryGuid;

    public ProductDao()
    {

    }
    public ProductDao(IProduct product)
    {
        Id = product.Id;
        Guid = product.Guid.ToDatabaseStringFormat();
        CategoryGuid = product.CategoryId.HasValue ? product.CategoryId.Value.ToDatabaseStringFormat() : null;
        UserGuid = product is UserProduct userProduct ? userProduct.UserID.ToDatabaseStringFormat() : null;
        Name = product.Name;
    }

    public int Id { get; set; }
    public string Guid { get; set; }
    public string? CategoryGuid { get => string.IsNullOrEmpty(CustomizedCategoryGuid) ? categoryGuid : CustomizedCategoryGuid; set => categoryGuid = value; }
    public string? UserGuid { get; set; }
    public string Name { get => string.IsNullOrEmpty(CustomizedName) ? name : CustomizedName; set => name = value; }
    public string? CustomizedCategoryGuid { get; set; }
    public string CustomizedName { get; set; }
}
