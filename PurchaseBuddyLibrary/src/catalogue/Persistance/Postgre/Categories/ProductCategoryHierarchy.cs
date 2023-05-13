namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

public class ProductCategoryHierarchy
{
    public int Id { get; set; }
    public Guid? UserGuid { get; set; }
    public Guid CategoryGuid { get; set; }
    public List<CategoryHierarchyNode> HierarchyNodes { get; }
    public class CategoryHierarchyNode
    {
        public CategoryHierarchyNode(int level, Guid guid)
        {
            Level = level;
            Guid = guid;
        }

        public int Level { get; set; }
        public Guid Guid { get; set; }
    }
    public static ProductCategoryHierarchy LoadFrom(ProductCategoryHierarchyDao dao)
    {
        var userGuid = string.IsNullOrEmpty(dao.UserGuid) ? (Guid?)null : Guid.Parse(dao.UserGuid);
        List<CategoryHierarchyNode> hierarchyNodes = GetNodesFrom(dao.RootPath);
        return new ProductCategoryHierarchy(dao.Id, userGuid, Guid.Parse(dao.CategoryGuid), hierarchyNodes);
    }

    private static List<CategoryHierarchyNode> GetNodesFrom(string rootPath)
    {
        var result = new List<CategoryHierarchyNode>();
        var nodes = rootPath.Split("/");
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];
            if (string.IsNullOrEmpty(node.Trim()))
                continue;

            result.Add(new CategoryHierarchyNode(i, Guid.Parse(nodes[i])));
        }

        return result;
    }

    private ProductCategoryHierarchy(int id, Guid? userGuid, Guid categoryGuid, List<CategoryHierarchyNode> hierarchyNodes)
    {
        Id = id;
        UserGuid = userGuid;
        CategoryGuid = categoryGuid;
        HierarchyNodes = hierarchyNodes;
    }
}
