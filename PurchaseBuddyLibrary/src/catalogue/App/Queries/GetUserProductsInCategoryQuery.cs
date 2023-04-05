namespace PurchaseBuddyLibrary.src.catalogue.App.Queries;

public class GetUserProductsInCategoryQuery
{
	public Guid UserId;
	public Guid CategoryId;

	public GetUserProductsInCategoryQuery(Guid userId, Guid categoryId)
	{
		UserId = userId;
		CategoryId = categoryId;
	}
}
