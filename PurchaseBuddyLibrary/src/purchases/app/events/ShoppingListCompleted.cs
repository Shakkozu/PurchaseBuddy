using MediatR;

namespace PurchaseBuddyLibrary.src.purchases.app.events;
public record ShoppingCompleted(Guid ListId, Guid CreatorId) : IRequest;
