namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

public class GetUserProductsInCategoryQuery
{
    public Guid UserId { get; }
    public Guid CategoryId { get; }

    public GetUserProductsInCategoryQuery(Guid userId, Guid categoryId)
    {
        UserId = userId;
        CategoryId = categoryId;
    }
}
