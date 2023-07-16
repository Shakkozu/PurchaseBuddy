using Microsoft.AspNetCore.Mvc.Routing;
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
		serviceCollection.AddTransient<IUserAuthorizationService, AuthorizationService>();
		serviceCollection.AddTransient<UserShopService, UserShopService>();
		serviceCollection.AddTransient<IUserShopService, UserShopService>();
		serviceCollection.AddTransient<IUserProductCategoriesManagementService, UserProductCategoriesManagementService>();
		serviceCollection.AddTransient<IShopCategoryListManagementService, ShopCategoryListManagementService>();
		serviceCollection.AddTransient<IShoppingListReadService, ShoppingListReadService>();
		serviceCollection.AddTransient<IShoppingListWriteService, ShoppingListWriteService>();
		serviceCollection.AddTransient<IUserProductsManagementService, UserProductsManagementService>();
		serviceCollection.AddTransient<UserProductsManagementService>();
		serviceCollection.AddTransient<CategoryFacade>();
		serviceCollection.AddTransient<ShoppingListSharingFacade>();
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
		serviceCollection.AddTransient<IUserRepository>(serviceProvider => new UserRepository(connectionString));
		serviceCollection.AddTransient<IUserShopRepository>(serviceProvider => new ShopsRepository(connectionString));
		serviceCollection.AddTransient<IProductsRepository>(serviceProvider => new ProductsRepository(connectionString));
		serviceCollection.AddTransient<ISharedProductRepository>(serviceProvider => new SharedProductRepository(connectionString));
		serviceCollection.AddTransient<IUserProductCategoriesRepository>(serviceProvider => new ProductCategoriesRepository(connectionString));
		serviceCollection.AddTransient<IShoppingListRepository>(serviceProvider => new ShoppingListRepository(connectionString));
		serviceCollection.AddTransient<ISharedShoppingListRepository>(serviceProvider =>  new SharedShoppingListRepository(connectionString));
	}
}
