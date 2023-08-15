using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddyLibrary.src.crm;

public class UsersProvider : IUsersProvider
{
	private readonly IUserRepository userRepository;

	public UsersProvider(IUserRepository userRepository)
	{
		this.userRepository = userRepository;
	}

	public IEnumerable<UserDto> GetAllUsersWithGuids(IEnumerable<Guid> guids)
	{
		return userRepository
			.GetAll()
			.Where(x => guids.Contains(x.Guid))
			.Select(user => new UserDto
			{
				Guid = user.Guid,
				Name = user.Login
			});
	}

	public IEnumerable<UserDto> GetAllUsers()
	{
		return userRepository
			.GetAll()
			.Select(user => new UserDto
			{
				Guid = user.Guid,
				Name = user.Login
			});
	}
}