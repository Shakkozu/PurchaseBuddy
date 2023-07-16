using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.persistance;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
using PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;
using PurchaseBuddyLibrary.src.stores.app;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.API;

public static class PurchaseBuddyFixture
{
	public static void RegisterDependencies(IServiceCollection serviceCollection, string connectionString)
	{
		RegisterRepositories(serviceCollection, connectionString);
		serviceCollection.AddSingleton<IUserAuthorizationService, AuthorizationService>();
		serviceCollection.AddSingleton<UserShopService, UserShopService>();
		serviceCollection.AddSingleton<IUserShopService, UserShopService>();
		serviceCollection.AddSingleton<IUserProductCategoriesManagementService, UserProductCategoriesManagementService>();
		serviceCollection.AddSingleton<IShopCategoryListManagementService, ShopCategoryListManagementService>();
		serviceCollection.AddSingleton<IShoppingListReadService, ShoppingListReadService>();
		serviceCollection.AddSingleton<IShoppingListWriteService, ShoppingListWriteService>();
		serviceCollection.AddSingleton<IUserProductsManagementService, UserProductsManagementService>();
		serviceCollection.AddSingleton<UserProductsManagementService>();
		serviceCollection.AddSingleton<CategoryFacade>();
		serviceCollection.AddSingleton<ShoppingListSharingFacade>();
        serviceCollection.AddTransient<ITimeService, TimeService>();
    }

	private static void RegisterRepositories(IServiceCollection serviceCollection, string connectionString)
	{
		if (string.IsNullOrEmpty(connectionString))
			RegisterInMemoryRepositories(serviceCollection);
		else
			RegisterRelationalRepositories(serviceCollection, connectionString);

	}
	private static void RegisterInMemoryRepositories(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IUserRepository>(new InMemoryUserRepository());
		serviceCollection.AddSingleton<IUserShopRepository>(new InMemoryUserShopRepository());
		serviceCollection.AddSingleton<IProductsRepository>(new InMemoryProductsRepository());
		serviceCollection.AddSingleton<IUserProductCategoriesRepository>(new InMemoryUserProductCategoriesRepository());
		serviceCollection.AddSingleton<IShoppingListRepository>(new InMemoryShoppingListRepository());
		serviceCollection.AddSingleton<ISharedShoppingListRepository>(new InMemorySharedShoppingListRepository());
	}
	private static void RegisterRelationalRepositories(IServiceCollection serviceCollection, string connectionString)
	{
		serviceCollection.AddSingleton<IUserRepository>(new UserRepository(connectionString));
		serviceCollection.AddSingleton<IUserShopRepository>(new ShopsRepository(connectionString));
		serviceCollection.AddSingleton<IProductsRepository>(new ProductsRepository(connectionString));
		serviceCollection.AddSingleton<ISharedProductRepository>(new SharedProductRepository(connectionString));
		serviceCollection.AddSingleton<IUserProductCategoriesRepository>(new ProductCategoriesRepository(connectionString));
		serviceCollection.AddSingleton<IShoppingListRepository>(new ShoppingListRepository(connectionString));
		serviceCollection.AddSingleton<ISharedShoppingListRepository>(new SharedShoppingListRepository(connectionString));
	}
}
