using PurchaseBuddyLibrary.src.auth.model;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.auth.persistance;

public class UserDao
{
	public static UserDao From(User user)
	{
		return new UserDao
		{
			Id = user.Id,
			Guid = user.Guid.ToDatabaseStringFormat(),
			Email = user.Email,
			Login = user.Login,
			PasswordHash = user.PasswordHash,
			IsAdministrator = user.IsAdministrator,
			Salt = user.Salt
		};
	}

	public User ToUser()
	{
		return User.LoadFrom(this);
	}

    public int Id { get; set; }
    public string? Guid { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string? Salt { get; set; }

	public string? PasswordHash { get; set; }
	public bool? IsAdministrator { get; set; }
}

