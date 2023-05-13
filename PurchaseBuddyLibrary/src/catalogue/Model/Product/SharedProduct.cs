using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using System.Collections.ObjectModel;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;

public class SharedProduct : IProduct
{
	public int Id { get; }
	public Guid Guid { get; }
	public Guid? CategoryId { get; private set; }
	public string Name { get; set; }

	private List<SharedProductCustomization> customizations { get; } = new List<SharedProductCustomization>();

	public static SharedProduct CreateNew(string name)
	{
		return new SharedProduct(name, Guid.NewGuid());
	}

	public void AssignProductToCategory(IProductCategory category)
	{
		CategoryId = category.Guid;
	}
	public void RemoveProductCategory()
	{
		CategoryId = null;
	}

	internal static IProduct LoadFrom(ProductDao sharedProductDao)
	{
		return new SharedProduct(sharedProductDao.Name,
			Guid.Parse(sharedProductDao.Guid),
			string.IsNullOrEmpty(sharedProductDao.CategoryGuid) ? (Guid?)null : Guid.Parse(sharedProductDao.CategoryGuid)
			);
	}

	private SharedProduct(string name, Guid guid, Guid? categoryId = null)
	{
		Name = name;
		Guid = guid;
		CategoryId = categoryId;
	}
}
