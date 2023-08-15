using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddyLibrary.src.purchases.app.events;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.eventHandlers;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;

namespace PurchaseBuddy.src.purchases.app;

public static class SharingModule
{
	public static void RegisterDependencies(IServiceCollection serviceCollection, string connectionString)
	{
		serviceCollection.AddTransient<IRequestHandler<ShoppingCompleted>, ShoppingCompletedEventHandler>();
		serviceCollection.AddTransient<SharingFacade>();
		serviceCollection.AddTransient<SharingReadFacade>();

		RegisterRepositories(serviceCollection, connectionString);
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
		serviceCollection.AddSingleton<IShoppingListInvitationsRepository>(new InMemoryShoppingListInvitationsRepository());
	}
	private static void RegisterRelationalRepositories(IServiceCollection serviceCollection, string connectionString)
	{
		serviceCollection.AddTransient<IShoppingListInvitationsRepository>(serviceProvider => new ShoppingListInvitationsRepository(connectionString));
	}
}
