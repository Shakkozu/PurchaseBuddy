using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.App.Queries;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductManagementTests : CatalogueTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
		productService = new UserProductsManagementService(userProductsRepo, userProductCategoriesService);
		queryHandler = new GetUserProductsQueryHandler(userProductsRepo, userProductCategoriesService);
	}

	[Test]
	public void DefineNewUserProduct_WhenProductDoesNotHaveSpecifiedCategory()
	{
		var productDto = AUserProduct();

		var createdId = productService.DefineNewUserProduct(productDto, UserId);

		var products = queryHandler.Handle(new GetUserProductsQuery(UserId));
		Assert.AreEqual(1, products.Count());
	}

	[Test]
	public void DefineNewUserProduct_WhenProductHasSpecifiedCategory()
	{
		var productCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productDto = AUserProduct(categoryId: productCategory);

		var createdId = productService.DefineNewUserProduct(productDto, UserId);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product);
		Assert.NotNull(product.Name);
		Assert.NotNull(product.CategoryId);
		Assert.IsNotEmpty(product.CategoryName);
	}

	[Test]
	public void ChangeProductCategory_WhenProductDoesNotHaveAssignedCategory()
	{
		var productCategoryId = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productDto = AUserProduct();
		var createdProduct = productService.DefineNewUserProduct(productDto, UserId);

		productService.AssignProductToCategory(UserId, createdProduct.Guid, productCategoryId);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.IsNotEmpty(product.CategoryName);
	}
	
	[Test]
	public void ChangeProductCategory_WhenProductHasAssignedCategory()
	{
		var originalCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test1"));
		var destCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test2"));
		var productDto = AUserProduct(categoryId: originalCategory);
		var createdProduct = productService.DefineNewUserProduct(productDto, UserId);

		productService.AssignProductToCategory(UserId, createdProduct.Guid, destCategory);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("test2", product.CategoryName);
		Assert.AreEqual(destCategory, product.CategoryId);
	}
	
	[Test]
	public void ChangeProductCategory_AssertUserCanChangeSharedProductForHimself()
	{
		var sharedProduct = SharedProduct.CreateNew("testSharedProduct");
		userProductsRepo.Save(sharedProduct);
		var originalCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test1"));
		var destCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test2"));
		productService.AssignProductToCategory(UserId, sharedProduct.Guid, originalCategory);

		productService.AssignProductToCategory(UserId, sharedProduct.Guid, destCategory);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("test2", product.CategoryName);
		Assert.AreEqual(destCategory, product.CategoryId);
	}
	
	[Test]
	public void ChangeProductCategory_AssertUserCanReassignSharedProductForHimself()
	{
		var sharedProduct = SharedProduct.CreateNew("testSharedProduct");
		userProductsRepo.Save(sharedProduct);
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test1"));

		productService.AssignProductToCategory(UserId, sharedProduct.Guid, category);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("test1", product.CategoryName);
		Assert.AreEqual(category, product.CategoryId);
	}

	[Test]
	public void DefineNewUserProduct_WhenProductHasSpecifiedCategoryAsSubcategory()
	{
		var productCategoryParent = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productCategoryChild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(parentId: productCategoryParent));
		var productDto = AUserProduct(categoryId: productCategoryChild);

		var createdId = productService.DefineNewUserProduct(productDto, UserId);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product);
		Assert.NotNull(product.Name);
		Assert.NotNull(product.CategoryId);
		Assert.IsNotEmpty(product.CategoryName);
	}
	
	[Test]
	public void DefineNewUserProduct_WhenProductHasSpecifiedCategoryAsSubSubcategory()
	{
		var productCategoryParent = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest());
		var productCategoryChild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(parentId: productCategoryParent));
		var productCategoryGrandchild = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(parentId: productCategoryChild, description: "dupa123", name:"testNAme"));
		var productDto = AUserProduct(categoryId: productCategoryGrandchild);

		var createdId = productService.DefineNewUserProduct(productDto, UserId);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product);
		Assert.AreEqual("testNAme", product.CategoryName);
		Assert.AreEqual(productCategoryGrandchild, product.CategoryId);
	}
	
	[Test]
	public void DefineNewUserProduct_WhenProductCategoryDoesNotExists_ThrowError()
	{
		var productDto = AUserProduct(categoryId: Guid.NewGuid());

		Assert.Throws<ResourceNotFoundException>(() => productService.DefineNewUserProduct(productDto, UserId));
	}

	private UserProductDto AUserProduct(string name = "test", Guid? categoryId = null)
	{
		return new UserProductDto
		{
			Name = name,
			CategoryId = categoryId,
		};
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
	private GetUserProductsQueryHandler queryHandler;
}
