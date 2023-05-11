using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddyLibrary.src.auth.model;
public class User
{
	public Guid Guid { get; }
	public int Id { get; }
	public string Email { get; }
	public string Login { get; }
	public string Salt { get; }
	public string PasswordHash { get; }

	internal static User CreateNew(string login, string email, string passwordHash, string salt)
	{
		return new User(null, Guid.NewGuid(), login, email, passwordHash, salt);
	}

	internal static User LoadFrom(UserDao userDao)
	{
		return new User(userDao.Id, Guid.Parse(userDao.Guid), userDao.Login, userDao.Email, userDao.PasswordHash, userDao.Salt);
	}

	private User(int? id, Guid guid, string login, string email, string passwordHash, string salt)
	{
		Guid = guid;
		Login = login;
		Email = email;
		PasswordHash = passwordHash;
		Salt = salt;
	}
}