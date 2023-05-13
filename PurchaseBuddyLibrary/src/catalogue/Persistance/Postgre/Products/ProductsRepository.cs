using Dapper;
using Npgsql;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;

public class ProductsRepository : IProductsRepository
{
    private readonly string connectionString;
    private readonly SharedProductRepository customizationRepo;

    public ProductsRepository(string connectionString)
    {
        this.connectionString = connectionString;
        customizationRepo = new SharedProductRepository(connectionString);
    }
    public IProduct? GetProduct(Guid productGuid, Guid userId)
    {
        var getUserProductSql = @"select 
id, guid, category_guid as CategoryGuid, user_guid as UserGuid, name from user_products
where guid like @ProductGuid";
        const string getSharedProductSql = @"select 
sp.id, sp.guid, sp.category_guid, sp.name, spc.category_guid as CustomizedCategoryGuid, spc.name as CustomizedName
from shared_products sp
left join shared_products_customization spc on sp.guid = spc.product_guid
where sp.guid like @ProductGuid and (spc.user_guid is null or spc.user_guid like @userId)";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var userProductDao = connection.QuerySingleOrDefault<ProductDao>(getUserProductSql, new { ProductGuid = productGuid.ToDatabaseStringFormat() });
            if (userProductDao != null)
                return UserProduct.LoadFrom(userProductDao);

            var sharedProductDao = connection.QuerySingleOrDefault<ProductDao>(getSharedProductSql, new
            {
                ProductGuid = productGuid.ToDatabaseStringFormat(),
                UserId = userId.ToDatabaseStringFormat()
            });
            if (sharedProductDao != null)
                return SharedProduct.LoadFrom(sharedProductDao);


            return null;
        }
    }

    public List<IProduct> GetSharedProducts()
    {
        throw new NotImplementedException();
    }

    public List<IProduct> GetUserProducts(Guid userID)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string getUserProductsSql = @"select id, guid, name, category_guid as CategoryGuid, user_guid as UserGuid from user_products where user_guid like @userId";
            const string getUserSharedProductsSQL = @"select 
sp.id, sp.guid, sp.category_guid, sp.name, spc.category_guid as CustomizedCategoryGuid, spc.name as CustomizedName
from shared_products sp
left join shared_products_customization spc on sp.guid = spc.product_guid
where spc.user_guid is null or spc.user_guid like @userId";
            var userProducts = connection.Query<ProductDao>(getUserProductsSql, new { UserId = userID.ToDatabaseStringFormat() });
            var sharedProducts = connection.Query<ProductDao>(getUserSharedProductsSQL, new { UserId = userID.ToDatabaseStringFormat() });
            var result = new List<IProduct>();
            if (userProducts != null)
                result.AddRange(userProducts.Select(x => UserProduct.LoadFrom(x)));
            if (sharedProducts != null)
                result.AddRange(sharedProducts.Select(x => SharedProduct.LoadFrom(x)));

            return result;
        }
    }

    public IProduct Save(IProduct product)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var productDao = new ProductDao(product);
            const string sql = @"insert into user_products (guid, category_guid, user_guid, name) values (
@Guid, @CategoryGuid, @UserGuid, @Name)";
            connection.ExecuteScalar(sql, new
            {
                productDao.Guid,
                productDao.CategoryGuid,
                productDao.UserGuid,
                product.Name
            });

            return product;
        }
    }

    public void SaveSharedProductCustomization(SharedProductCustomization customization)
    {
        const string addCustomizationSql = @"insert into shared_products_customization (product_guid, user_guid, category_guid, name)
values (@ProductGuid,
@UserGuid,
@CategoryGuid,
@Name)";
        const string clearCurrentCustomizationSql = @"delete from shared_products_customization where user_guid like @UserGuid and product_guid like @ProductGuid";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.ExecuteScalar(clearCurrentCustomizationSql, new
            {
                UserGuid = customization.UserID.ToDatabaseStringFormat(),
                ProductGuid = customization.ProductGuid.ToDatabaseStringFormat()
            });

            connection.ExecuteScalar(addCustomizationSql, new
            {
                ProductGuid = customization.ProductGuid.ToDatabaseStringFormat(),
                UserGuid = customization.UserID.ToDatabaseStringFormat(),
                CategoryGuid = customization.CategoryId.HasValue ? customization.CategoryId.Value.ToDatabaseStringFormat() : null,
                customization.Name
            });
        }
    }

    public void Update(IProduct product, Guid userId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            if (product is UserProduct)
            {
                var productDao = new ProductDao(product);
                const string sql = @"update user_products set
	category_guid = @CategoryGuid,
	name = @Name
	where guid like @Guid";
                connection.ExecuteScalar(sql, new
                {
                    product.Name,
                    productDao.CategoryGuid,
                    productDao.Guid
                });
            }
            if (product is SharedProduct)
            {
                var customization = new SharedProductCustomization(product.Guid, userId, product.Name, product.CategoryId);
                SaveSharedProductCustomization(customization);
            }
        }
    }
}
