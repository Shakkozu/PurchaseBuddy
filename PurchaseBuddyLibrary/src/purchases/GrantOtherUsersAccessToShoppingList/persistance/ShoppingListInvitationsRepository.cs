using Dapper;
using Npgsql;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.persistance;

public class ShoppingListInvitationsRepository : IShoppingListInvitationsRepository
{
	private readonly string connectionString;
	public ShoppingListInvitationsRepository(string connectionString)
	{
		this.connectionString = connectionString;
	}

	public ShoppingInvitationsList? Get(Guid shoppingListGuid)
	{
		const string sqlQuery = @"
select 
	guid,
	shopping_creator_guid as CreatorId,
	shopping_list_guid as ListId,
	is_active as IsActive,
	invited_users as UsersInvitedToModify,
	allowed_users as UsersAllowedToModify
from shopping_invitations
where shopping_list_guid like @ShoppingListGuid";
		using (var dbConnection = new NpgsqlConnection(connectionString))
		{
			var result = dbConnection.QueryFirstOrDefault<ShoppingInvitationsListDao>(sqlQuery, new
			{
				ShoppingListGuid = shoppingListGuid.ToDatabaseStringFormat()
			});

			if (result == null)
				return null;

			return ShoppingInvitationsList.LoadFrom(result);
		}
	}

	public IEnumerable<ShoppingInvitationsList> GetAllWhereUserIsOnInvitationsList(Guid userGuid)
	{
		const string sqlQuery = @"
select 
	guid,
	shopping_creator_guid as CreatorId,
	shopping_list_guid as ListId,
	is_active as IsActive,
	invited_users as UsersInvitedToModify,
	allowed_users as UsersAllowedToModify
from shopping_invitations
where invited_users like @InvitedUsersQuery";
		using (var dbConnection = new NpgsqlConnection(connectionString))
		{
			var result = dbConnection.Query<ShoppingInvitationsListDao>(sqlQuery, new
			{
				InvitedUsersQuery = $"%{userGuid.ToDatabaseStringFormat()}%"
			});

			if (result == null || !result.Any())
				return new List<ShoppingInvitationsList>();

			return result.Select(entry => ShoppingInvitationsList.LoadFrom(entry));
		}
	}

	public void Save(ShoppingInvitationsList shoppingList)
	{
		const string sqlQuery = @"
insert into shopping_invitations
	(guid, 
	shopping_creator_guid,
	shopping_list_guid,
	is_active,
	invited_users,
	allowed_users)
values
	(
	@Guid,
	@CreatorId,
	@ListId,
	@IsActive,
	@UsersInvitedToModify,
	@UsersAllowedToModify
	)";
		using (var dbConnection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShoppingInvitationsListDao(shoppingList);
			dbConnection.Execute(sqlQuery, new
			{
				dao.Guid,
				dao.CreatorId,
				dao.ListId,
				dao.IsActive,
				dao.UsersInvitedToModify,
				dao.UsersAllowedToModify
			});
		}
	}

	public void Update(ShoppingInvitationsList shoppingList)
	{
		const string sqlQuery = @"
update shopping_invitations
set
	(is_active = @IsActive,
	invited_users = @UsersInvitedToModify
	allowed_users = @UsersAllowedToModify)
where guid like @Guid";
		using (var dbConnection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShoppingInvitationsListDao(shoppingList);
			dbConnection.Execute(sqlQuery, new
			{
				dao.Guid,
				dao.IsActive,
				dao.UsersInvitedToModify,
				dao.UsersAllowedToModify
			});
		}
	}
}
