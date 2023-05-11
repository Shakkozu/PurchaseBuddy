using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProductCategories;

public class GetUserProductCategoriesQueryHandler
{
    private readonly IUserProductCategoriesRepository productCategoriesRepository;

    public GetUserProductCategoriesQueryHandler(IUserProductCategoriesRepository productCategoriesRepository)
    {
        this.productCategoriesRepository = productCategoriesRepository;
    }
    public GetUserProductCategoriesResponse Handle(Guid userId)
    {
        var productCategories = productCategoriesRepository.FindAll(userId)
            .Where(cat => cat.IsRoot)
            .ToList();

        var dtos = new List<ProductCategoryDto>();
        foreach (var category in productCategories)
            dtos.Add(new ProductCategoryDto(category));

        return new GetUserProductCategoriesResponse(dtos);
    }
}