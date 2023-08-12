using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;

public class ShoppingInvitationsListDao
{
	public ShoppingInvitationsListDao()
	{

	}


	internal ShoppingInvitationsListDao(ShoppingInvitationsList invitations)
	{
		CreatorId = invitations.CreatorId.ToDatabaseStringFormat();
		ListId = invitations.ListId.ToDatabaseStringFormat();
		IsActive = invitations.IsActive;
		Guid = invitations.Guid.ToDatabaseStringFormat();
		UsersAllowedToModify = string.Join(UsersSeparator,
			invitations.UsersAllowedToModify.Select(x => x.ToDatabaseStringFormat()));
		UsersInvitedToModify = string.Join(UsersSeparator,
			invitations.UsersInvitedToModify.Select(x => x.ToDatabaseStringFormat()));
	}

	public string ListId { get; set; }
	public string CreatorId { get; set; }
	public bool IsActive { get; set; }
	public string UsersAllowedToModify { get; set; }
	public string UsersInvitedToModify { get; set; }
	public string Guid { get; internal set; }

	internal List<Guid> GetUsersAllowedToModify()
	{
		if (string.IsNullOrEmpty(UsersAllowedToModify))
			return new List<Guid>();

		var users = UsersAllowedToModify.Split(UsersSeparator, StringSplitOptions.TrimEntries);
		return users.Select(System.Guid.Parse).ToList();
	}
	internal List<Guid> GetUsersInvitedToModify()
	{
		if (string.IsNullOrEmpty(UsersInvitedToModify))
			return new List<Guid>();

		var users = UsersInvitedToModify.Split(UsersSeparator, StringSplitOptions.TrimEntries);
		return users.Select(System.Guid.Parse).ToList();
	}

	private const string UsersSeparator = ",";
}
