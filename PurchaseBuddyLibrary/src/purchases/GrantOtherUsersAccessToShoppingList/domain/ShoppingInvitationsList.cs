using System.Collections.ObjectModel;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.domain;

public class Invitation
{
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class ShoppingInvitationsList
{
    private readonly List<Guid> _usersAllowedToModify;
    private readonly List<Invitation> _usersInvitedToModify;

    public bool IsCompleted { get; private set; }
    public Guid ListId { get; private set; }
    public Guid Guid { get; private set; }
    public Guid CreatorId { get; private set; }

	public IReadOnlyCollection<Invitation> UsersInvitedToModify => new ReadOnlyCollection<Invitation>(_usersInvitedToModify);

	public IReadOnlyCollection<Guid> UsersAllowedToModify => new ReadOnlyCollection<Guid>(_usersAllowedToModify);

    public bool IsActive { get; private set; }

    public static ShoppingInvitationsList CreateNew(Guid listId, bool listCompleted, Guid creatorId)
    {
        if (listCompleted)
            throw new InvalidOperationException("Cannot invite user to already completed list");

        return new ShoppingInvitationsList(
            listId,
            true,
            Guid.NewGuid(),
            creatorId,
            new List<Invitation>(),
            new List<Guid>());
    }

	public Invitation? GetInvitationForUser(Guid user)
	{
		return _usersInvitedToModify?.Find(invite => invite.UserId == user);
	}
    public static ShoppingInvitationsList LoadFrom(ShoppingInvitationsListDao dao)
    {
        return new ShoppingInvitationsList(Guid.Parse(dao.ListId),
            dao.IsActive,
            Guid.Parse(dao.Guid),
            Guid.Parse(dao.CreatorId),
            dao.GetUsersInvitedToModify(),
            dao.GetUsersAllowedToModify());
    }
    private ShoppingInvitationsList(Guid listId,
        bool isActive,
        Guid guid,
        Guid listCreatorId,
        List<Invitation> usersInvitedToModify,
        List<Guid> usersAllowedToModify)
    {
        Guid = guid;
        CreatorId = listCreatorId;
        ListId = listId;
        IsActive = isActive;
        _usersInvitedToModify = usersInvitedToModify ?? new List<Invitation>();
        _usersAllowedToModify = usersAllowedToModify ?? new List<Guid>();
    }

    internal void MarkAsExpired()
    {
        IsActive = false;
    }

    internal void InviteUser(Guid otherUser, Guid listCreatorId)
    {
        if (!IsActive)
            throw new InvalidOperationException("Inviting other users to modyfing a list is not permitted anymore");
        if (listCreatorId != CreatorId)
            throw new InvalidOperationException("Only creator can invite other users");
        if (_usersInvitedToModify.Exists(user => user.UserId == otherUser))
            return;

        _usersInvitedToModify.Add(new Invitation
		{
			UserId = otherUser,
			CreatedAt = DateTime.UtcNow,
		});
    }

    internal void AcceptInviteToModify(Guid invitedUser)
    {
        if (!IsActive)
            throw new InvalidOperationException("Resource has expired");

        if (_usersAllowedToModify.Contains(invitedUser))
            return;

        if (!_usersInvitedToModify.Exists(user => user.UserId == invitedUser))
            throw new InvalidOperationException($"User with guid: {invitedUser} is not invited to modify list");

        _usersAllowedToModify.Add(invitedUser);
		var invitationToRemove = _usersInvitedToModify.Single(invite => invite.UserId == invitedUser);
        _usersInvitedToModify.Remove(invitationToRemove);
    }

    internal void Reject(Guid invitedUser)
    {
        if (_usersAllowedToModify.Contains(invitedUser))
            _usersAllowedToModify.Remove(invitedUser);
        if (_usersInvitedToModify.Exists(user => user.UserId == invitedUser))
		{
			var invitationToRemove = _usersInvitedToModify.Single(invite => invite.UserId == invitedUser);
			_usersInvitedToModify.Remove(invitationToRemove);
		}
    }
}
