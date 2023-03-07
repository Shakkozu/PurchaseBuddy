using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.catalogue.contract;

public class ProductCategoryDto
{
	public string Name { get; set; }
	public string? Description { get; set; }
	public Guid Guid { get; set; }

	public List<ProductCategoryDto> Children { get; set; }
	public ProductCategoryDto()
	{

	}

	public ProductCategoryDto(IProductCategory category)
	{
		Name = category.Name;
		Guid = category.Guid;
		Children = category.Children.Select(child => new ProductCategoryDto(child)).ToList();
		Description = category.Description;
	}
}
