namespace PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

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
