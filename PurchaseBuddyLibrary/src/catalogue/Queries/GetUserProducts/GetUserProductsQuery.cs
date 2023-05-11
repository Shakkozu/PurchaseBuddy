using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;
public class GetUserProductsQuery
{
    public GetUserProductsQuery(Guid userId, string? filter = null, int page = 1, int pageSize = 10)
    {
        UserId = userId;
        Filter = filter;
        Page = page;
        PageSize = pageSize;
    }
    public string? Filter { get; }
    public Guid UserId { get; }
    public int Page { get; }
    public int PageSize { get; }
}

public class GetUserProductsQueryHandler
{
    private readonly IProductsRepository productsRepository;
    private readonly IUserProductCategoriesManagementService userProductCategoriesManagementService;

    public GetUserProductsQueryHandler(IProductsRepository productsRepository,
        IUserProductCategoriesManagementService userProductCategoriesManagementService)
    {
        this.productsRepository = productsRepository;
        this.userProductCategoriesManagementService = userProductCategoriesManagementService;
    }

    public List<UserProductDto> Handle(GetUserProductsQuery query)
    {
        var itemsToSkip = (query.Page - 1) * query.PageSize;

        return productsRepository.GetUserProducts(query.UserId)
            .Where(x => string.IsNullOrEmpty(query.Filter) || x.Name.ToLower().Contains(query.Filter.ToLower()))
            .OrderByDescending(x => x.Name.ToLower())
            .Skip(itemsToSkip)
            .Take(query.PageSize)
            .Select(p => p.CategoryId.HasValue
                ? new UserProductDto(p, userProductCategoriesManagementService.GetUserProductCategory(query.UserId, p.CategoryId.Value))
                : new UserProductDto(p))
            .ToList();
    }
}
