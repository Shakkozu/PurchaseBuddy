using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductCategoryManagementTests : CatalogueTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);
	}

	public class TestValidation
	{
		private TestValidation(int value) {
			Value = value;
		}

		public static TestValidation CreateFrom(int? input)
		{
			return input switch
			{
				null => throw new ArgumentNullException(),
				<= 0 => throw new ArgumentOutOfRangeException(),
				_ => new TestValidation(input.Value),
			};
		}
        public int Value { get; }
    }

	[Test]
	public void CanAddNewProductCategory()
	{
		var createRequest = AUserProductCategoryCreateRequest();

		var createdId = userProductCategoriesService.AddNewProductCategory(UserId, createRequest);

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		Assert.True(productCategories.Any(category => category.Guid == createdId));
	}

	[Test]
	public void UserCanAddNewProductCategory_WhenNewCategoryIsOnRootLeve()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
	}

	[Test]
	public void UserCanAddNewProductCategory_WhenNewCategoryHasParent()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root));
		userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child2", root));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(2, productCategories.First().Children.Count);
	}
	
	[Test]
	public void UserCanAddNewProductCategory_WhenNewCategoryIsOnLevel3OfIndent()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root));
		var child2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child2", child));

		var productCategories = userProductCategoriesService.GetCategories(UserId);
		var lastAddedChild = productCategories.First().Children.First().Children.First();

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(1, productCategories.First().Children.Count);
		Assert.NotNull(lastAddedChild);
	}
	
	[Test]
	public void WhenUserReassignsSubcategory_AssertChildSubSubcategoriesAreAlsoReassigned()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root1"));
		var root2= userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child", root));
		var grandChild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("grandChild", child));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.IsEmpty(productCategories.First(pc => pc.Guid == root).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children.First().Children);
	}
	
	[Test]
	public void UserCanReassignCategory_WhenCategoryAlreadyHasParent()
	{
		var root1 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root1"));
		var root2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1", root1));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(2, productCategories.Count);
		Assert.IsEmpty(productCategories.First(pc => pc.Guid == root1).Children);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
	}
	
	[Test]
	public void UserCanReassignCategory_WhenNewParentCategoryIsSharedCategory()
	{
		var root = ASharedCategory("dairy");
		userCategoriesRepo.Save(root);
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child"));

		userProductCategoriesService.ReassignCategory(UserId, child, root.Guid);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.IsNotEmpty(productCategories.First().Children);
	}

	[Test]
	public void UserCanReassignCategory_WhenCategoryDoNotHaveParent()
	{
		var root2 = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("root2"));
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("child1"));

		userProductCategoriesService.ReassignCategory(UserId, child, root2);
		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.IsNotEmpty(productCategories.First(pc => pc.Guid == root2).Children);
	}

	[Test]
	public void GetUserCategories_AssertThereAreNoDuplicates()
	{
		var root = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var child = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("test", root));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.NotNull(productCategories.First().Children);
	}
	
	[Test]
	public void GetUserCategories_WhenThereAreBothSharedAndUserCategories_AssertThereAreNoDuplicates()
	{
		var root = ASharedCategory("dairy");
		var child = ASharedCategory("milk", root);
		userCategoriesRepo.Save(root);
		userCategoriesRepo.Save(child);

		var childUser = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest("test", root.Guid));

		var productCategories = userProductCategoriesService.GetCategories(UserId);

		Assert.AreEqual(1, productCategories.Count);
		Assert.AreEqual(2,productCategories.First(c => c.Guid == root.Guid).Children.Count);
		Assert.NotNull(productCategories.First().Children);
	}

	[Test]
	public void CreateProductAndAssignToCategory()
	{
		var productCategory = ProductCategoryCreated("dairy");

		var productDto = new UserProductDto
		{
			CategoryId = productCategory.Guid,
			Name = "Cheese"
		};

		productService.DefineNewUserProduct(productDto, UserId);

		var categoryFromDb = userProductCategoriesService.GetCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
	}

	private IProductCategory ProductCategoryCreated(string name)
	{
		var category = UserProductCategory.CreateNew(name, UserId);
		return userCategoriesRepo.Save(category);
	}

	private IProduct ProductCreated(string name)
	{
		var product = UserProduct.Create(name, UserId);
		return userProductsRepo.Save(product);
	}


	private InMemoryProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;
}
