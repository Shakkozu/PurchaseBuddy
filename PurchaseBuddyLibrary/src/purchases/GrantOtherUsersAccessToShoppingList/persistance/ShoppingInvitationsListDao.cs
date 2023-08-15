using System;
using Newtonsoft.Json;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;



internal class InvitationDao
{
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

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
		var invitationDtos = invitations.UsersInvitedToModify.Select(x => new InvitationDao
		{
			CreatedAt = x.CreatedAt,
			UserId = x.UserId
		});
		UsersInvitedToModify = JsonConvert.SerializeObject(invitationDtos, Formatting.Indented);
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
	internal List<Invitation> GetUsersInvitedToModify()
	{
		if (string.IsNullOrEmpty(UsersInvitedToModify))
			return new List<Invitation>();

		var daos = JsonConvert.DeserializeObject<List<InvitationDao>>(UsersInvitedToModify);
		return daos.Select(x => new Invitation
		{
			CreatedAt = x.CreatedAt,
			UserId = x.UserId
		}).ToList();
	}

	private const string UsersSeparator = ",";
}
