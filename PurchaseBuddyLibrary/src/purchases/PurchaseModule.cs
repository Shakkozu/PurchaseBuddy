using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.events;

namespace PurchaseBuddyLibrary.src.purchases.app.eventHandlers;

public static class PurchaseModule
{
	public static void RegisterModule(IServiceCollection serviceCollection, string connectionString)
	{
		serviceCollection.AddTransient<IShoppingListReadService, ShoppingListReadService>();
		serviceCollection.AddTransient<IShoppingListWriteService, ShoppingListWriteService>();
		serviceCollection.AddTransient<IRequestHandler<InvitationToModifyingShoppingListAccepted>, GrantAccessToModifyingSharedListService>();

		SharingModule.RegisterDependencies(serviceCollection, connectionString);
	}
}