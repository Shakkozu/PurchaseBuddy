using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductsFactoryTests 
{
	private ProductCategoriesFactory factory;
	private string guid = "09004CA1-3950-4EBD-A09F-0637298D3E87";
	private string guid2 = "09004CA1-3950-4EBD-A09F-1234abcd0987";
	private string guid3 = "09004CA1-3950-4EBD-A09F-123412345987";

	[SetUp]
	public void SetUp()
	{
		factory = new ProductCategoriesFactory();
	}

	[Test]
	public void CanBuildSingleProductCategory()
	{
		var categoryDaoList = ADaoListWithSingleEntry();
		var singleDao = categoryDaoList.Single();

		var result = (UserProductCategory)factory.Load(categoryDaoList).First();

		Assert.AreEqual(singleDao.Name, result.Name);
		Assert.AreEqual(Guid.Parse(singleDao.Guid), result.Guid);
		Assert.AreEqual(Guid.Parse(singleDao.UserGuid), result.UserId);
		Assert.False(result.ParentId.HasValue);
		Assert.IsEmpty(result.Children);
	}

	[Test]
	public void CanBuildProductCategoryWithParent()
	{
		var categoryDaoList = ADaoListWithParent();

		var result = factory.Load(categoryDaoList);

		var parent = result.Single(cat => cat.Guid == Guid.Parse(guid));
		var child = result.Single(cat => cat.Guid == Guid.Parse(guid2));
		Assert.AreEqual(true, parent.IsRoot);
		Assert.AreEqual(child.ParentId, parent.Guid);
		Assert.AreEqual(child, parent.Children.First());
	}

	[Test]
	public void CanBuildProductCategoryWithMultipleChildren()
	{
		var categoryDaoList = ADaoListWithMultipleChildren();

		var result = factory.Load(categoryDaoList);

		var parent = result.Single(cat => cat.Guid == Guid.Parse(guid));
		var child1 = result.Single(cat => cat.Guid == Guid.Parse(guid2));
		var child2 = result.Single(cat => cat.Guid == Guid.Parse(guid3));
		Assert.AreEqual(true, parent.IsRoot);
		Assert.AreEqual(child1.ParentId, parent.Guid);
		Assert.AreEqual(child2.ParentId, parent.Guid);
		Assert.AreEqual(2, parent.Children.Count);
	}

	[Test]
	public void CanBuildProductCategoryWithNestedChildren()
	{
		var categoryDaoList = ADaoListWithNestedChildren();

		var result = factory.Load(categoryDaoList);

		var parent = result.Single(cat => cat.Guid == Guid.Parse(guid));
		var child = result.Single(cat => cat.Guid == Guid.Parse(guid2));
		var grandChild = result.Single(cat => cat.Guid == Guid.Parse(guid3));
		Assert.AreEqual(true, parent.IsRoot);
		Assert.AreEqual(child.ParentId, parent.Guid);
		Assert.AreEqual(grandChild.ParentId, child.Guid);
		Assert.AreEqual(1, parent.Children.Count);
		Assert.AreEqual(1, parent.Children.First().Children.Count);
		Assert.AreEqual(1, child.Children.Count);
		Assert.AreEqual(parent.Guid, grandChild.Parent?.ParentId);
	}

	private List<ProductCategoryDao> ADaoListWithNestedChildren()
	{
		return new List<ProductCategoryDao>
		{
			new ProductCategoryDao
			{
				Description = "description",
				Guid = guid,
				Hierarchy = "",
				Id = 1,
				Name = "parent",
				UserGuid = guid
			},
			new ProductCategoryDao
			{
				Guid = guid2,
				Hierarchy = guid,
				Id = 2,
				Name = "child",
				UserGuid = guid
			},
			new ProductCategoryDao
			{
				Guid = guid3,
				Hierarchy = guid + "/" + guid2,
				Id = 3,
				Name = "grandchild",
				UserGuid = guid
			},
		};
	}

	private List<ProductCategoryDao> ADaoListWithMultipleChildren()
	{
		return new List<ProductCategoryDao>
		{
			new ProductCategoryDao
			{
				Description = "description",
				Guid = guid,
				Hierarchy = "",
				Id = 1,
				Name = "parent",
				UserGuid = guid
			},
			new ProductCategoryDao
			{
				Guid = guid2,
				Hierarchy = guid,
				Id = 2,
				Name = "child",
				UserGuid = guid
			},
			new ProductCategoryDao
			{
				Guid = guid3,
				Hierarchy = guid,
				Id = 3,
				Name = "child2",
				UserGuid = guid
			},
		};
	}

	private List<ProductCategoryDao> ADaoListWithParent()
	{
		return new List<ProductCategoryDao>
		{
			new ProductCategoryDao
			{
				Description = "description",
				Guid = guid,
				Hierarchy = "",
				Id = 1,
				Name = "parent",
				UserGuid = guid
			},
			new ProductCategoryDao
			{
				Guid = guid2,
				Hierarchy = guid,
				Id = 2,
				Name = "child",
				UserGuid = guid
			},
		};
	}

	private List<ProductCategoryDao> ADaoListWithSingleEntry()
	{
		return new List<ProductCategoryDao>
		{
			new ProductCategoryDao
			{
				Description = "description",
				Guid = guid,
				Hierarchy = "",
				Id = 1,
				Name = "name",
				UserGuid = guid
			}
		};
	}
}
