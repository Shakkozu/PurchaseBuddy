using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;

namespace PurchaseBuddy.Tests.stores.integration;
internal class UserShopServiceTests
{
	private InMemoryUserShopRepository userShopRepository;
	private UserShopService userShopService;

	[SetUp]
	public void SetUp()
	{
		userShopRepository = new InMemoryUserShopRepository();
		userShopService = new UserShopService(userShopRepository);
	}

	[Test]
	public void GetUserShop_AssertCorrectIsReturned()
	{
		var addedShopId = userShopService.AddNewUserShop(Fixture.UserId, UserShopDescription.CreateNew("test"));

		Assert.NotNull(userShopService.GetUserShopById(Fixture.UserId, addedShopId));
	}

	[Test]
	public void GetAllUserShops_AssertAllUserShopsAreReturned()
	{
		userShopService.AddNewUserShop(Fixture.UserId, UserShopDescription.CreateNew("test"));
		userShopService.AddNewUserShop(Fixture.UserId, UserShopDescription.CreateNew("test2"));

		Assert.AreEqual(2, userShopService.GetAllUserShops(Fixture.UserId).Count);
	}
}
