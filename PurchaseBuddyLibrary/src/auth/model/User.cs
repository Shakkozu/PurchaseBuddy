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
		return new User(Guid.NewGuid(), login, email, passwordHash, salt);
	}

	private User(Guid guid, string login, string email, string passwordHash, string salt)
	{
		Guid = guid;
		Login = login;
		Email = email;
		PasswordHash = passwordHash;
		Salt = salt;
	}
}