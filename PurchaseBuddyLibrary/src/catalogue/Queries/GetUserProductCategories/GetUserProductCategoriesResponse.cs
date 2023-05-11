namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProductCategories;

public record GetUserProductCategoriesResponse
{
    public GetUserProductCategoriesResponse(IEnumerable<ProductCategoryDto> categories)
    {
        Categories = categories;
    }
    public IEnumerable<ProductCategoryDto> Categories { get; set; } = new List<ProductCategoryDto>();
}


