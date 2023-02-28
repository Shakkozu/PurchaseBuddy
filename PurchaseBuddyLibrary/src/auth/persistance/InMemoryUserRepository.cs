using PurchaseBuddyLibrary.src.auth.model;

namespace PurchaseBuddyLibrary.src.auth.persistance;

public class InMemoryUserRepository : IUserRepository
{
    private Dictionary<Guid, User> cache = new();
    public void Add(User user)
    {
        if (cache.ContainsKey(user.Guid))
            throw new ArgumentException("User with this guid already exists");

        cache.Add(user.Guid, user);
    }

    public User? GetByEmail(string email)
    {
        return cache.Values.FirstOrDefault(u => u.Email == email);
    }

    public User GetByGuid(Guid guid)
    {
        if (cache.ContainsKey(guid))
            return cache[guid];

        throw new ArgumentException("User with this guid does not exist");
    }

    public User? GetByLogin(string login)
    {
        return cache.Values.FirstOrDefault(u => u.Login == login);
    }
}
