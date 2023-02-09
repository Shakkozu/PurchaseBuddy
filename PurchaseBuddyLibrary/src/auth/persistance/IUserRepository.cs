using PurchaseBuddyLibrary.src.auth.model;

namespace PurchaseBuddyLibrary.src.auth.persistance;
public interface IUserRepository
{
	public User? GetByLogin(string login);
	public User? GetByEmail(string email);
	public User GetByGuid(Guid guid);
	public void Add(User user);
}
