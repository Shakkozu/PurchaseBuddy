using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.API;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddy.Tests.purchases.Integration;

internal class SharingInvitationFlowIntergationTests : PurchaseBuddyTestsFixture
{
	private IShoppingListWriteService shoppingListWriteService;
	private IShoppingListReadService shoppingListReadService;

	public Guid OtherUser { get; private set; }

	private SharingReadFacade sharingReadFacade;
	private SharingFacade sharingFacade;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var services = new ServiceCollection();
		PurchaseBuddyFixture.RegisterDependencies(services, TestConfigurationHelper.GetConnectionString());
		var serviceProvider = services.BuildServiceProvider();
		shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();
		shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();
		UserId = AUserCreated();
		OtherUser = ANewUserCreated();
		sharingReadFacade = serviceProvider.GetRequiredService<SharingReadFacade>();
		sharingFacade = serviceProvider.GetRequiredService<SharingFacade>();
	}

	[Test]
	public void SharingFlowWorksCorrectly()
	{
		//given
		var shoppingListId = AShoppingListWithSingleItemCreated();

		// when
		sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

		// then
		var invitation = sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser).First();
		Assert.NotNull(invitation);

		//when
		sharingFacade.AcceptAnInvite(invitation.ListId, OtherUser);
		//then
		Assert.IsEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser));
		Assert.NotNull(shoppingListReadService.GetShoppingList(OtherUser, shoppingListId));
	}

	[Test]
	public void InteractingWithSharedListWorksCorrectly()
	{
		//given
		var shoppingListId = AShoppingListWithSingleItemCreated();
		shoppingListWriteService.AddNewListItem(UserId, shoppingListId, new AddNewListItemRequest { ProductName = "Chocolate" });
		sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);
		sharingFacade.AcceptAnInvite(shoppingListId, OtherUser);
		var listItems = shoppingListReadService.GetShoppingList(OtherUser, shoppingListId).ShoppingListItems;

		shoppingListWriteService.MarkListItemAsPurchased(OtherUser, shoppingListId, Guid.Parse(listItems.First().Guid));
		shoppingListWriteService.MarkListItemAsPurchased(OtherUser, shoppingListId, Guid.Parse(listItems.Last().Guid));

		var shoppingList = shoppingListReadService.GetShoppingList(OtherUser, shoppingListId);
		Assert.IsTrue(shoppingList.Completed);
	}

	private Guid AShoppingListWithSingleItemCreated()
	{
		return shoppingListWriteService.CreateNewList(UserId, new List<ShoppingListItem> { ImportedShoppingListItem.CreateNew("banana", "food")});
	}
}
