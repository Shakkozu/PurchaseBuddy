using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.Database;
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

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		var configBuilder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			var configuration = configBuilder.Build();
		var databaseConnectionString = configuration.GetConnectionString("Database");
		app = PurchaseBuddyApp.CreateInstance(databaseConnectionString);
		sessionId = CreateAndLogUser();
		httpClient = app.CreateClient();
		httpClient.DefaultRequestHeaders.Add("Authorization", sessionId.ToString());
	}

	[SetUp]
    public void SetUp()
    {
        
    }

    [OneTimeTearDown]
    public async Task DisposeOfApp()
    {
		var servicesCollection = new ServiceCollection();
		var configBuilder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		var configuration = configBuilder.Build();
		var databaseConnectionString = configuration.GetConnectionString("Database");

		MigrationsRunner.ClearDatabase(servicesCollection, databaseConnectionString);
		await app.DisposeAsync();
    }

    private Guid CreateAndLogUser()
    {
        app.AuthorizationService.Register(AUser());
        return app.AuthorizationService.Login(AUser().Login, AUser().Password);
    }
}
