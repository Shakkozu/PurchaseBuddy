using PurchaseBuddyLibrary.src.auth.model;
using Dapper;
using Npgsql;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.auth.persistance;

public class UserRepository : IUserRepository
{
	private readonly string connectionString;

	public UserRepository(string connectionString)
    {
		this.connectionString = connectionString;
	}
    public void Add(User user)
	{
		const string sql = 
@"INSERT INTO users (guid, email, login, salt, password_hash)
 values (@Guid, @Email, @Login, @Salt, @PasswordHash)";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			connection.Execute(sql, UserDao.From(user));
		}
	}

	public User? GetByEmail(string email)
	{
		var sql = @"select guid, email, login, salt, password_hash, is_administrator as IsAdministrator from users where email like @Email";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var result = connection.QuerySingleOrDefault<UserDao>(sql, new { Email = email});
			if (result == null)
				return null;

			return result.ToUser();
		}
	}

	public User GetByGuid(Guid guid)
	{
		var sql = @"select guid, email, login, salt, password_hash, is_administrator as IsAdministrator from users where guid like @Guid";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var result = connection.QuerySingleOrDefault<UserDao>(sql, new { Guid = guid.ToDatabaseStringFormat() });
			if (result == null)
				throw new ResourceNotFoundException("user with guid" + guid + "not found");

			return result.ToUser();
		}
	}

	public User? GetByLogin(string login)
	{
		var sql = @"select guid, email, login, salt, password_hash as PasswordHash, is_administrator as IsAdministrator from users where login like @Login";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var result = connection.QuerySingleOrDefault<UserDao>(sql, new { Login = login });
			if (result == null)
				return null;

			return result.ToUser();
		}
	}

	public void GrantAdministratorAccessRights(Guid userId)
	{
		const string sql = @"update users set is_administrator = true where guid like @UserGuid";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			connection.ExecuteScalar(sql, new { UserGuid = userId.ToDatabaseStringFormat() });
		}
	}
}

