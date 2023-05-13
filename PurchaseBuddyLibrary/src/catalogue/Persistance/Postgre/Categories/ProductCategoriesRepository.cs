using Dapper;
using Npgsql;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

public class ProductCategoriesRepository : IUserProductCategoriesRepository
{
    private readonly string connectionString;
    private readonly ProductHierarchyRepository hierarchyRepository;
    private readonly ProductCategoriesFactory productsFactory;

    public ProductCategoriesRepository(string connectionString)
    {
        this.connectionString = connectionString;
        hierarchyRepository = new ProductHierarchyRepository(connectionString);
        productsFactory = new ProductCategoriesFactory();
    }
    public List<IProductCategory> FindAll(Guid userId)
    {
        return productsFactory.Load(GetAllCategoriesForUser(userId));
    }

    public IProductCategory? FindById(Guid userId, Guid categoryGuid)
    {
        return productsFactory
            .Load(GetAllCategoriesForUser(userId))
            .FirstOrDefault(c => c.Guid == categoryGuid);
    }
    private List<ProductCategoryDao> GetAllCategoriesForUser(Guid userId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string sql = @"select pc.id, pc.guid, pc.user_guid as UserGuid, pc.name, pc.description, root_path as Hierarchy
from product_categories pc
join product_categories_hierarchy pch on  pch.category_guid like pc.guid";
            return connection.Query<ProductCategoryDao>(sql,
                new
                {
                    UserGuid = userId.ToDatabaseStringFormat()
                }).ToList();
        }

    }

    public void Remove(IProductCategory category)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            hierarchyRepository.Delete(category);
            const string sql = @"delete from product_categories where guid like @Guid";
            connection.ExecuteScalar(sql,
                new
                {
                    Guid = category.Guid.ToDatabaseStringFormat(),
                });
        }
    }

    public IProductCategory Save(IProductCategory productCategory)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string sql = @"insert into product_categories (guid, user_guid, name, description) values (@Guid, @UserGuid, @Name, @Description) returning id";
            var userProductCategory = productCategory as UserProductCategory;
            var userGuid = userProductCategory != null ? userProductCategory.UserId : (Guid?)null;

            connection.ExecuteScalar(sql,
                new
                {
                    Guid = productCategory.Guid.ToDatabaseStringFormat(),
                    UserGuid = userGuid.HasValue ? userGuid.Value.ToDatabaseStringFormat() : null,
                    productCategory.Name,
                    productCategory.Description,
                });

        }
        hierarchyRepository.Save(productCategory);
        return productCategory;
    }

    public void Update(IProductCategory productCategory)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            const string updateCategorySql = @"update product_categories set 
name = @Name,
description = @Description
where guid = @Guid";
            hierarchyRepository.Update(productCategory);
        }
    }
}
