using MediatR;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.events;
public record InvitationToModifyingShoppingListAccepted(DateTime Timestamp, Guid ListId, Guid UserId) : IRequest;
