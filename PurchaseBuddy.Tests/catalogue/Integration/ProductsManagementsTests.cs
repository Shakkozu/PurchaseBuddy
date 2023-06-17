using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PurchaseBuddy.Database;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

namespace PurchaseBuddy.Tests.catalogue.Integration;

internal class ProductManagementTests : PurchaseBuddyTestsFixture
{
	private IProductsRepository productsRepo;
	private IUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductCategoriesService;
	private UserProductsManagementService productService;
	private GetUserProductsQueryHandler queryHandler;
	private AddNewSharedProductCommandHandler addSharedProductCommandHandler;
	private AuthorizationService authorizationService;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		var services = new ServiceCollection();
		var connectionString = TestConfigurationHelper.GetConnectionString();
		MigrationsRunner.ClearDatabase(services, connectionString);
		MigrationsRunner.RunMigrations(services, connectionString);

		productsRepo = new ProductsRepository(connectionString);
		userCategoriesRepo = new ProductCategoriesRepository(connectionString);
		userProductCategoriesService = new UserProductCategoriesManagementService(userCategoriesRepo, productsRepo);
		productService = new UserProductsManagementService(productsRepo, userProductCategoriesService);
		queryHandler = new GetUserProductsQueryHandler(productsRepo, userProductCategoriesService);
		authorizationService = new AuthorizationService(new UserRepository(connectionString), new ConfigurationBuilder().Build());
		var userRepository = new UserRepository(connectionString);
		var sharedProductsRepo = new SharedProductRepository(connectionString);
		addSharedProductCommandHandler = new AddNewSharedProductCommandHandler(sharedProductsRepo, userRepository);
		UserId = AUserCreated();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		ClearDatabase();
	}

	private static void ClearDatabase()
	{
		using (var connection = new NpgsqlConnection(TestConfigurationHelper.GetConnectionString()))
		{
			connection.Execute("delete from shared_products_customization");
			connection.Execute("delete from user_products");
			connection.Execute("delete from shared_products");
			connection.Execute("delete from product_categories");
		}
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

		productService.ChangeProductCategory(UserId, createdProduct.Guid, productCategoryId);

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

		productService.ChangeProductCategory(UserId, createdProduct.Guid, destCategory);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("test2", product.CategoryName);
		Assert.AreEqual(destCategory, product.CategoryId);
	}
	
	[Test]
	public void ChangeProductCategory_AssertUserCanChangeSharedProductForHimself()
	{
		authorizationService.GrantAdministratorAccessRights(UserId);
		var sharedProductGuid = addSharedProductCommandHandler.Handle(new AddNewSharedProductCommand { Name = "testSharedProduct", UserGuid = UserId });
		var originalCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test1"));
		var destCategory = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test2"));
		productService.ChangeProductCategory(UserId, sharedProductGuid, originalCategory);

		productService.ChangeProductCategory(UserId, sharedProductGuid, destCategory);

		var product = queryHandler.Handle(new GetUserProductsQuery(UserId)).First();
		Assert.NotNull(product.CategoryId);
		Assert.AreEqual("test2", product.CategoryName);
		Assert.AreEqual(destCategory, product.CategoryId);
	}

	[Test]
	public void ChangeProductCategory_AssertUserCanReassignSharedProductForHimself()
	{
		authorizationService.GrantAdministratorAccessRights(UserId);
		var sharedProductGuid = addSharedProductCommandHandler.Handle(new AddNewSharedProductCommand { Name = "testSharedProduct", UserGuid = UserId });
		var category = userProductCategoriesService.AddNewProductCategory(UserId, AUserProductCategoryCreateRequest(name: "test1"));

		productService.ChangeProductCategory(UserId, sharedProductGuid, category);

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
}
