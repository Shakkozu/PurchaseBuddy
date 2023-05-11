using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre;

public class ProductCategoriesFactory
{
    public List<IProductCategory> Load(List<ProductCategoryDao> categories)
    {
        var result = GetCategories(categories);
		FillHierarchiesInfo(result, categories);

        return result;
    }

    private static void FillHierarchiesInfo(List<IProductCategory> result, List<ProductCategoryDao> categories)
    {
        foreach (var category in result)
        {
            var hierarchy = categories.First(c => Guid.Parse(c.Guid) == category.Guid).Hierarchy;
            if (string.IsNullOrWhiteSpace(hierarchy))
                continue;

            var parentGuid = Guid.Parse(hierarchy.Split("/").Last());
            var parent = result.First(c => c.Guid == parentGuid);
            parent.AddChild(category);
        }
    }

    private static List<IProductCategory> GetCategories(List<ProductCategoryDao> categories)
    {
        var result = new List<IProductCategory>();
        foreach (var category in categories)
        {
            if (!string.IsNullOrWhiteSpace(category.UserGuid))
            {
                result.Add(UserProductCategory.LoadFrom(
                    category.Id,
                    Guid.Parse(category.Guid),
                    Guid.Parse(category.UserGuid),
                    category.Name,
                    category.Description
                    ));
            }
            else
            {
                result.Add(SharedProductCategory.LoadFrom(
                    category.Id,
                    Guid.Parse(category.Guid),
                    category.Name,
                    category.Description
                    ));
            }
        }

        return result;
    }
}