using MediatR;
using Microsoft.Extensions.Logging;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.src.purchases.app.events;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.eventHandlers;

internal class ShoppingCompletedEventHandler : IRequestHandler<ShoppingCompleted>
{
    private readonly SharingFacade sharingFacade;
    private readonly ILogger<ShoppingCompletedEventHandler> logger;

    public ShoppingCompletedEventHandler(SharingFacade sharingFacade,
        ILogger<ShoppingCompletedEventHandler> logger)
    {
        this.sharingFacade = sharingFacade;
        this.logger = logger;
    }
    public Task Handle(ShoppingCompleted notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling event ShoppingCompleted {@notification}", notification);
        sharingFacade.MarkInvitationsAsExpired(notification.ListId);
        return Task.CompletedTask;
    }
}
