using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;

namespace PurchaseBuddyLibrary.src.catalogue.App.Queries;

public class GetUserProductsInCategoryQueryHandler
{
	private IProductsRepository productsRepository;
	private UserProductCategoriesManagementService userProductCategoriesManagementService;

	public GetUserProductsInCategoryQueryHandler(IProductsRepository productsRepository,
	UserProductCategoriesManagementService userProductCategoriesManagementService)
	{
		this.productsRepository = productsRepository;
		this.userProductCategoriesManagementService = userProductCategoriesManagementService;
	}

	public List<UserProductDto> Handle(GetUserProductsInCategoryQuery query)
	{
		return productsRepository.GetUserProducts(query.UserId)
			.Where(x => x.CategoryId == query.CategoryId)
			.Select(p => new UserProductDto(p, userProductCategoriesManagementService.GetUserProductCategory(query.UserId, p.CategoryId.Value)))
			.ToList();
	}
}
