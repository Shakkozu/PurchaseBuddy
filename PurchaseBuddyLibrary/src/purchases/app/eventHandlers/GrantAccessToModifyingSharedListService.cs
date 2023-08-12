using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.events;

namespace PurchaseBuddyLibrary.src.purchases.app.eventHandlers;

internal class GrantAccessToModifyingSharedListService : IRequestHandler<InvitationToModifyingShoppingListAccepted>
{
    private readonly IShoppingListWriteService shoppingListWrite;
    private readonly ILogger<GrantAccessToModifyingSharedListService> logger;

    public GrantAccessToModifyingSharedListService(IShoppingListWriteService shoppingListWrite,
        ILogger<GrantAccessToModifyingSharedListService> logger)
    {
        this.shoppingListWrite = shoppingListWrite;
        this.logger = logger;
    }
    public Task Handle(InvitationToModifyingShoppingListAccepted request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling event InvitationToModifyingShoppingListAccepted {@notification}", request);
        shoppingListWrite.GrantAccessToModifyingList(request.ListId, request.UserId);
        return Task.CompletedTask;
    }
}
public static class PurchaseModule
{
	public static void RegisterModule(IServiceCollection serviceCollection)
	{
		serviceCollection.AddTransient<IShoppingListReadService, ShoppingListReadService>();
		serviceCollection.AddTransient<IShoppingListWriteService, ShoppingListWriteService>();
		serviceCollection.AddTransient<IRequestHandler<InvitationToModifyingShoppingListAccepted>, GrantAccessToModifyingSharedListService>();
	}
}