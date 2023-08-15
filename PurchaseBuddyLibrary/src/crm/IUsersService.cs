namespace PurchaseBuddyLibrary.src.crm;

public interface IUsersProvider
{
	IEnumerable<UserDto> GetAllUsers();
	IEnumerable<UserDto> GetAllUsersWithGuids(IEnumerable<Guid> guids);
}
