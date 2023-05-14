using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using PurchaseBuddyLibrary.src.stores.app;

namespace PurchaseBuddy.API;

public static class PurchaseBuddyFixture
{
	public static void RegisterDependencies(IServiceCollection serviceCollection, string connectionString)
	{
		serviceCollection.AddSingleton<IUserRepository>(new UserRepository(connectionString));
		serviceCollection.AddSingleton<IUserAuthorizationService, AuthorizationService>();
		serviceCollection.AddSingleton<UserShopService, UserShopService>();
		serviceCollection.AddSingleton<IUserShopRepository>(new ShopsRepository(connectionString));
		serviceCollection.AddSingleton<IUserShopService, UserShopService>();
		serviceCollection.AddSingleton<IProductsRepository>(new ProductsRepository(connectionString));
		serviceCollection.AddSingleton<ISharedProductRepository>(new SharedProductRepository(connectionString));
		serviceCollection.AddSingleton<IUserProductCategoriesRepository>(new ProductCategoriesRepository(connectionString));
		serviceCollection.AddSingleton<IUserProductCategoriesManagementService, UserProductCategoriesManagementService>();
		serviceCollection.AddSingleton<IShopCategoryListManagementService, ShopCategoryListManagementService>();
		serviceCollection.AddSingleton<IShoppingListService, ShoppingListProductsManagementService>();
		serviceCollection.AddSingleton<IShoppingListRepository>(new ShoppingListRepository(connectionString));
		serviceCollection.AddSingleton<IUserProductsManagementService, UserProductsManagementService>();
		serviceCollection.AddSingleton<UserProductsManagementService>();
		serviceCollection.AddSingleton<CategoryFacade>();
	}
}
