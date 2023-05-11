using Dapper;
using Npgsql;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.utils;
using System.Text;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre;

internal class ProductHierarchyRepository
{
    private readonly string connectionString;
    private const string TableName = "product_categories_hierarchy";
    public ProductHierarchyRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    internal void Save(IProductCategory productCategory)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var userProductCategory = productCategory as UserProductCategory;
            var userGuid = userProductCategory != null ? userProductCategory.UserId : (Guid?)null;
            var sql = string.Format(@"insert into {0} (category_guid, user_guid, root_path) values (@CategoryGuid, @UserGuid, @RootPath)", TableName);
            connection.ExecuteScalar(sql,
                new
                {
                    CategoryGuid = productCategory.Guid,
                    UserGuid = userGuid.HasValue ? userGuid.Value.ToDatabaseStringFormat() : null,
                    RootPath = GetRootPathForProductCategory(productCategory)
                });

            return;
        }
    }
    private string GetRootPathForProductCategory(IProductCategory productCategory)
    {
        var result = new StringBuilder();
        if (productCategory.ParentId.HasValue)
            result.Append(productCategory.ParentId.Value.ToDatabaseStringFormat());

        return result.ToString();
    }

    internal void Update(IProductCategory productCategory)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string updateHierarchySql = @"update product_categories_hierarchy set 
root_path = @Hierarchy
where category_guid = @CategoryGuid";
            connection.ExecuteScalar(updateHierarchySql,
                new
                {
                    Hierarchy = GetRootPathForProductCategory(productCategory),
                    CategoryGuid = productCategory.Guid.ToDatabaseStringFormat(),
                });
        }
    }

    internal void Delete(IProductCategory category)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string deleteHierarchySql = @"delete from product_categories_hierarchy
where category_guid = @CategoryGuid";
            connection.ExecuteScalar(deleteHierarchySql,
                new
                {
                    CategoryGuid = category.Guid.ToDatabaseStringFormat(),
                });
        }
    }
}
