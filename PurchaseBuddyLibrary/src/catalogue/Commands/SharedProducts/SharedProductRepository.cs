using Dapper;
using Npgsql;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
public interface ISharedProductRepository
{
	void Save(SharedProduct product);
}

public class SharedProductRepository : ISharedProductRepository
{
    private readonly string connectionString;

    public SharedProductRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void Save(SharedProduct product)
    {
        const string sql = @"insert into shared_products (guid, name, category_guid) 
values
(@Guid,
@Name,
@CategoryGuid)";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.ExecuteScalar(sql, new
            {
                Guid = product.Guid.ToDatabaseStringFormat(),
                product.Name,
                CategoryGuid = product.CategoryId.HasValue ? product.CategoryId.Value.ToDatabaseStringFormat() : null
            });
        }
    }
}