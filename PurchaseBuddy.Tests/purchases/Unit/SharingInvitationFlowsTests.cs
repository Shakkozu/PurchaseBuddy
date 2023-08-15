using FakeItEasy;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.API;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.crm;
using PurchaseBuddyLibrary.src.purchases.app.contract;
using PurchaseBuddyLibrary.src.purchases.GrantOtherUsersAccessToShoppingList.events;

namespace PurchaseBuddy.Tests.purchases.Unit;
internal class SharingInvitationFlowsTests : PurchaseBuddyTestsFixture
{
    private IMediator mediator;
    private SharingFacade sharingFacade;
    private SharingReadFacade sharingReadFacade;
    private IShoppingListReadService shoppingListReadService;
    private IShoppingListWriteService shoppingListWriteService;

	public Guid OtherUser { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();
        PurchaseBuddyFixture.RegisterDependencies(services, null);
        var usersProvider = new UsersProvider(new UserRepository(TestConfigurationHelper.GetConnectionString()));
        services.AddSingleton<IUsersProvider>(usersProvider);
        var serviceProvider = services.BuildServiceProvider();

        shoppingListReadService = serviceProvider.GetRequiredService<IShoppingListReadService>();
        shoppingListWriteService = serviceProvider.GetRequiredService<IShoppingListWriteService>();
        sharingFacade = serviceProvider.GetRequiredService<SharingFacade>();
        sharingReadFacade = serviceProvider.GetRequiredService<SharingReadFacade>();
        UserId = AUserCreated();
        OtherUser = ANewUserCreated();
        var shoppingListInvitationsRepository = serviceProvider.GetRequiredService<IShoppingListInvitationsRepository>();
        mediator = A.Fake<IMediator>();

        sharingReadFacade = new SharingReadFacade(shoppingListInvitationsRepository, usersProvider);
        sharingFacade = new SharingFacade(shoppingListInvitationsRepository, shoppingListReadService, mediator);
    }

    [Test]
    public void ShouldCreateInvitationForNotCompletedShoppingList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();

        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

        Assert.IsNotEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser));
    }

    [Test]
    public void ShouldCreateManyInvitesForSingleList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        var otherUser2 = ANewUserCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, otherUser2);

        Assert.IsNotEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(otherUser2));
    }

    [Test]
    public void ShouldNotCreateAnInvitationForCompletedShoppingList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        var shoppingList = shoppingListReadService.GetShoppingList(UserId, shoppingListId);
        shoppingListWriteService.MarkListItemAsPurchased(UserId, shoppingListId, Guid.Parse(shoppingList.ShoppingListItems.First().Guid));

        Assert.Throws<InvalidOperationException>(() => sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser));
    }


    [Test]
    public void AcceptingActiveInviteShouldSendEvent()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

        sharingFacade.AcceptAnInvite(shoppingListId, OtherUser);

        A.CallTo(() => mediator.Send(
            A<InvitationToModifyingShoppingListAccepted>.That.Matches(invite => invite.UserId == OtherUser && invite.ListId == shoppingListId),
            default))
            .MustHaveHappened();
    }

    [Test]
    public void AcceptingActiveInviteShouldRemovItFromActiveInvitesList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

        sharingFacade.AcceptAnInvite(shoppingListId, OtherUser);

        Assert.IsEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser));
    }

    [Test]
    public void AcceptingSameInviteMultipleTimesDoesNotThrowAnyException()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);
        sharingFacade.AcceptAnInvite(shoppingListId, OtherUser);

        Assert.DoesNotThrow(() => sharingFacade.AcceptAnInvite(shoppingListId, OtherUser));
    }

    [Test]
    public void RejectingAnInvitationShouldRemoveItFromPendingInvitationsList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);

        sharingFacade.RejectAnInvite(shoppingListId, OtherUser);

        Assert.IsEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser));
    }

    [Test]
    public void MarkingAnInvitationListShouldRemoveAllInvitesFromActiveInvitesList()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        var otherUser2 = ANewUserCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, otherUser2);

        sharingFacade.MarkInvitationAsExpired(shoppingListId);

        Assert.IsEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(OtherUser));
        Assert.IsEmpty(sharingReadFacade.GetAllActiveInvitationsForUser(otherUser2));
    }

    [Test]
    public void InvitingDifferentUserToModificationNotByCreatorIsNotAllowed()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        var otherUser2 = ANewUserCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);
        sharingFacade.AcceptAnInvite(shoppingListId, OtherUser);

        Assert.Throws<ArgumentException>(() => sharingFacade.InviteOtherUserToModifyList(OtherUser, shoppingListId, otherUser2));
    }

    [Test]
    public void UserShouldNotBeAbleToAcceptExpiredInvitation()
    {
        var shoppingListId = AShoppingListWithSingleItemCreated();
        sharingFacade.InviteOtherUserToModifyList(UserId, shoppingListId, OtherUser);
        sharingFacade.MarkInvitationAsExpired(shoppingListId);

        Assert.Throws<InvalidOperationException>(() => sharingFacade.AcceptAnInvite(shoppingListId, OtherUser));
    }


    private Guid AShoppingListWithSingleItemCreated()
    {
        return shoppingListWriteService.CreateNewList(UserId, new List<ShoppingListItem> { ImportedShoppingListItem.CreateNew("banana", "food") });
    }
}
