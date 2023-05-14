using Dapper;
using Npgsql;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.src.purchases.persistance;

public class ShoppingListRepository : IShoppingListRepository
{
	public ShoppingListRepository(string connectionString)
	{
		this.connectionString = connectionString;
	}

	public IList<ShoppingList> GetAll(Guid userId)
	{
		const string sql = @"select 
guid,
user_guid as UserGuid,
shop_guid as ShopGuid,
created_at as CreatedAt,
completed_at as CompletedAt,
items as ItemsString
from shopping_lists
where user_guid like @UserGuid";

		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = connection.Query<ShoppingListDao>(sql, new
			{
				UserGuid = userId.ToDatabaseStringFormat()
			});
			if (dao == null || !dao.Any())
				return new List<ShoppingList>();

			return dao
				.Select(entry => ShoppingList.LoadFrom(entry))
				.ToList();
		}
	}

	public ShoppingList? GetShoppingList(Guid userId, Guid shoppingListGuid)
	{
		const string sql = @"select 
guid,
user_guid as UserGuid,
shop_guid as ShopGuid,
created_at as CreatedAt,
completed_at as CompletedAt,
items as ItemsString
from shopping_lists
where guid like @Guid and user_guid like @UserGuid";

		using(var connection = new NpgsqlConnection(connectionString))
		{
			var dao = connection.QueryFirstOrDefault<ShoppingListDao>(sql, new
			{
				UserGuid = userId.ToDatabaseStringFormat(),
				Guid = shoppingListGuid.ToDatabaseStringFormat()
			});
			if (dao == null)
				return null;

			return ShoppingList.LoadFrom(dao);
		}
	}

	public void Save(ShoppingList shoppingList)
	{
		const string sql = @"insert into shopping_lists (guid, user_guid, shop_guid, created_at, completed_at, items)
values
(@Guid,
@UserGuid,
@ShopGuid,
@CreatedAt,
@CompletedAt,
@Items)";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShoppingListDao(shoppingList);
			connection.ExecuteScalar(sql, new
			{
				Guid = dao.Guid,
				UserGuid = dao.UserGuid,
				ShopGuid = dao.ShopGuid,
				CreatedAt = dao.CreatedAt,
				CompletedAt = dao.CompletedAt,
				Items = dao.ItemsString
			});
		}
	}

	public void Update(ShoppingList shoppingList)
	{
		const string sql = @"update shopping_lists set
shop_guid = @ShopGuid,
completed_at = @CompletedAt,
items = @Items
where guid like @Guid";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShoppingListDao(shoppingList);
			connection.ExecuteScalar(sql, new
			{
				Guid = dao.Guid,
				ShopGuid = dao.ShopGuid,
				Items = dao.ItemsString,
				CompletedAt = dao.CompletedAt,
			});
		}
	}

	private readonly string connectionString;
}
