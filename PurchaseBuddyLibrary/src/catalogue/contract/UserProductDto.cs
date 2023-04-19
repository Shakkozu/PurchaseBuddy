using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.src.catalogue.App;

public class UserProductDto
{
    public UserProductDto()
    {
        
    }

    public UserProductDto(IProduct product, IProductCategory? productCategory = null)
    {
		Name = product.Name;
		Guid = product.Guid;
		if(productCategory != null)
		{
			CategoryId = productCategory.Guid;
			CategoryName = productCategory.Name;
		}
    }
    public Guid Guid { get; set; }
    public string Name { get; set; }
	public Guid? CategoryId { get; set; }
    public string? CategoryName  { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj == null) return false;
		if (obj == this) return true;
		if (obj.GetType() != GetType())
			return false;

		var other = (UserProductDto)obj;
		return other.Guid == Guid
			&& other.Name == Name
			&& other.CategoryId == CategoryId
			&& other.CategoryName == CategoryName;
	}
}