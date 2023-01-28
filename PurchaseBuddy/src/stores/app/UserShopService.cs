using PurchaseBuddy.src.stores.domain;

namespace PurchaseBuddy.src.stores.app;

public class UserShopService
{
	private readonly IUserShopRepository userShopRepository;

	public UserShopService(IUserShopRepository userShopRepository)
	{
		this.userShopRepository = userShopRepository;
	}

	public Guid AddNewUserShop(Guid userId, UserShopDescription userShopDescription)
	{
		var userShop = UserShop.CreateNew(userId, userShopDescription);
		userShopRepository.Save(userShop);

		return userShop.Guid;
	}
	
	public UserShop? GetUserShopById(Guid userId, Guid userShopId)
	{
		return userShopRepository.GetUserShop(userId, userShopId);
	}
	
	public List<UserShop> GetAllUserShops(Guid userId)
	{
		return userShopRepository.GetAllUserShops(userId);
	}
}
