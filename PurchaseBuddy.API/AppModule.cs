using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre;
using PurchaseBuddyLibrary.src.stores.app;
using PurchaseBuddyLibrary.src.stores.persistance;

namespace PurchaseBuddy.API;

public static class PurchaseBuddyFixture
{
	public static void RegisterDependencies(IServiceCollection serviceCollection, string connectionString)
	{
		//serviceCollection.AddSingleton<IUserRepository, InMemoryUserRepository>();
		serviceCollection.AddSingleton<IUserRepository>(new UserRepository(connectionString));
		serviceCollection.AddSingleton<IUserAuthorizationService, AuthorizationService>();
		serviceCollection.AddSingleton<UserShopService, UserShopService>();
		serviceCollection.AddSingleton<IUserShopRepository, InMemoryUserShopRepository>();
		serviceCollection.AddSingleton<IUserShopService, UserShopService>();
		serviceCollection.AddSingleton<IProductsRepository, InMemoryProductsRepository>();
		serviceCollection.AddSingleton<IUserProductCategoriesRepository>(new ProductCategoriesRepository(connectionString));
		//serviceCollection.AddSingleton<IUserProductCategoriesRepository, InMemoryUserProductCategoriesRepository>();
		serviceCollection.AddSingleton<IUserProductCategoriesManagementService, UserProductCategoriesManagementService>();
		serviceCollection.AddSingleton<IShopMapRepository, InMemoryShopMapRepository>();
		serviceCollection.AddSingleton<IShopCategoryListManagementService, ShopCategoryListManagementService>();
		serviceCollection.AddSingleton<IShoppingListService, ShoppingListProductsManagementService>();
		serviceCollection.AddSingleton<IShoppingListRepository, InMemoryShoppingListRepository>();
		serviceCollection.AddSingleton<IUserProductsManagementService, UserProductsManagementService>();
		serviceCollection.AddSingleton<UserProductsManagementService>();
		serviceCollection.AddSingleton<CategoryFacade>();
	}
}
