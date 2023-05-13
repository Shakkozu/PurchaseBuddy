using Dapper;
using Npgsql;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.src.stores.persistance;

public class ShopsRepository : IUserShopRepository
{
	private readonly string connectionString;

	public ShopsRepository(string connectionString)
    {
		this.connectionString = connectionString;
	}
    public void Delete(UserShop shop)
	{
		throw new NotImplementedException();
	}

	public List<UserShop> GetAllUserShops(Guid userId)
	{
		const string sql = @"select
guid,
user_guid as UserGuid,
name,
description,
city,
street,
local_number as LocalNumber,
configuration as ConfigurationString
from shops
where user_guid like @UserGuid";

		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = connection.Query<ShopDao>(sql, new
			{
				UserGuid = userId.ToDatabaseStringFormat()
			});

			if (dao == null || !dao.Any())
				return new List<UserShop>();

			return dao.Select(x => UserShop.LoadFrom(x))
				.ToList();
		}
	}

	public UserShop? GetUserShop(Guid userId, Guid userShopId)
	{
		const string sql = @"select
guid,
user_guid as UserGuid,
name,
description,
city,
street,
local_number as LocalNumber,
configuration as ConfigurationString
from shops
where user_guid like @UserGuid and guid like @ShopGuid";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = connection.QueryFirst<ShopDao>(sql, new
			{
				UserGuid = userId.ToDatabaseStringFormat(),
				ShopGuid = userShopId.ToDatabaseStringFormat()
			});

			if (dao == null)
				return null;

			return UserShop.LoadFrom(dao);
		}
	}


	public void Save(UserShop userShop)
	{
		const string sql = @"insert into shops
(guid, user_guid, name, description, city, street, local_number, configuration)
values (@Guid, @UserGuid, @Name, @Description, @City, @Street, @LocalNumber, @configuration)";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShopDao(userShop);
			connection.ExecuteScalar(sql, new
			{
				Guid = dao.Guid,
				UserGuid = dao.UserGuid,
				Name = dao.Name,
				Description = dao.Description,
				City = dao.City,
				Street = dao.Street,
				LocalNumber = dao.LocalNumber,
				Configuration = dao.ConfigurationString
			});
		}
	}

	void IUserShopRepository.Update(UserShop shop)
	{
		const string sql = @"update shops set
name = @Name,
description = @Description,
city = @City,
street = @Street,
local_number = @LocalNumber,
configuration = @configuration
where guid like @Guid";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = new ShopDao(shop);
			connection.ExecuteScalar(sql, new
			{
				Name = dao.Name,
				Description = dao.Description,
				City = dao.City,
				Street = dao.Street,
				LocalNumber = dao.LocalNumber,
				Configuration = dao.ConfigurationString,
				Guid = dao.Guid
			});
		}
	}
}
