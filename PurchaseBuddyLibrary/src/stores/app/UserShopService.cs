using PurchaseBuddy.src.infra;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;

namespace PurchaseBuddy.src.stores.app;

public class UserShopService : IUserShopService
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

	public void UpdateShopDescription(UserShopDescription userShopDescription, Guid userGuid, Guid shopId)
	{
		var shop = GetUserShopById(userGuid, shopId);
		if (shop == null)
			throw new ResourceNotFoundException("Shop not found");

		shop.ChangeDescriptionTo(userShopDescription);
		userShopRepository.Save(shop);
	}

	public void DeleteUserShop(Guid userGuid, Guid shopId)
	{
		var shop = GetUserShopById(userGuid, shopId);
		if (shop == null)
			throw new ResourceNotFoundException("Shop not found");

		userShopRepository.Delete(shop);
	}
}
