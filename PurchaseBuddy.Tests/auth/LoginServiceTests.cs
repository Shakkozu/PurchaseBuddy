using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddy.Tests.auth;

internal class LoginServiceTests : AuthorizationTestsFixture
{
    private InMemoryUserRepository userRepository;
    private AuthorizationService authorizationService;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        userRepository = new InMemoryUserRepository();
        authorizationService = new AuthorizationService(userRepository, Configuration);
    }

    [Test]
    public void WhenUserIsLoggedInCorrectly_AssertSessionIdIsReturned()
    {
        var user = AUser();
        authorizationService.Register(user);

        var result = authorizationService.Login(user.Login, user.Password);

        Assert.NotNull(result);
    }

    [TestCase("johnDoe", "invliadPassword")]
    [TestCase("invalidUsername", "zaq1@WSX")]
    public void WhenCredentialsAreInvalid_ThrowException(string username, string password)
    {
        var user = AUser();
        authorizationService.Register(user);

        Assert.Throws<ArgumentException>(() => authorizationService.Login(username, password));
    }
}
