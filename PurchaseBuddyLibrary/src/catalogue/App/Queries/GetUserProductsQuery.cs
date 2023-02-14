using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.catalogue.App.Queries;


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

	public GetUserProductsQueryHandler(IProductsRepository productsRepository)
	{
		this.productsRepository = productsRepository;
	}
	public List<IProduct> Handle(GetUserProductsQuery query)
	{
		var itemsToSkip = (query.Page - 1) * query.PageSize;

		return productsRepository.GetUserProducts(query.UserId)
			.Where(x => string.IsNullOrEmpty(query.Filter) || x.Name.ToLower().Contains(query.Filter.ToLower()))
			.OrderByDescending(x => x.Name.ToLower())
			.Skip(itemsToSkip)
			.Take(query.PageSize)
			.ToList();
	}
}
