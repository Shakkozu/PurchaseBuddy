using PurchaseBuddyLibrary.src.auth.contract;

namespace PurchaseBuddy.Tests;

internal abstract class ControllersTestsFixture
{
    protected Guid sessionId;
    protected PurchaseBuddyApp app;
    protected HttpClient httpClient;
    protected Fixtures Fixtures => app.Fixtures;

    protected UserDto AUser()
    {
        return new UserDto
        {
            Email = "john.doe@example.com",
            Login = "johnDoe",
            Password = "zaq1@WSX"
        };
    }

    [SetUp]
    public void SetUp()
    {
        app = PurchaseBuddyApp.CreateInstance();
        sessionId = CreateAndLogUser();
        httpClient = app.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", sessionId.ToString());
    }

    [TearDown]
    public async Task DisposeOfApp()
    {
        await app.DisposeAsync();
    }

    private Guid CreateAndLogUser()
    {
        app.AuthorizationService.Register(AUser());
        return app.AuthorizationService.Login(AUser().Login, AUser().Password);
    }
}
