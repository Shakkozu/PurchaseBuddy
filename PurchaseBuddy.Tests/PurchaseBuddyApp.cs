using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PurchaseBuddy.API;
using PurchaseBuddyLibrary.src.auth.app;

namespace PurchaseBuddy.Tests;

internal class PurchaseBuddyApp : WebApplicationFactory<Program>
{
    private IServiceScope _scope;
    private readonly Action<IServiceCollection> _customization;
    private bool _reuseScope = false;

    private PurchaseBuddyApp(Action<IServiceCollection> customization)
    {
        _customization = customization;
        _scope = base.Services.CreateAsyncScope();
    }

    public static PurchaseBuddyApp CreateInstance(string? databaseConnectionString)
    {
        var app = new PurchaseBuddyApp(collection => {
			
		});
        return app;
    }

    public static PurchaseBuddyApp CreateInstance(Action<IServiceCollection> customization)
    {
        var purchaseBuddyApp = new PurchaseBuddyApp(customization);
        return purchaseBuddyApp;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
		var configBuilder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		var configuration = configBuilder.Build();
		var databaseConnectionString = configuration.GetConnectionString("Database");

		builder.ConfigureServices(collection => {
			collection.AddTransient<Fixtures>();
			PurchaseBuddyFixture.RegisterDependencies(collection, databaseConnectionString);

		}
		);
        builder.ConfigureServices(_customization);
    }

    public void StartReuseRequestScope()
    {
        _reuseScope = true;
    }

    public void EndReuseRequestScope()
    {
        _reuseScope = false;
    }

    protected override void Dispose(bool disposing)
    {
        _scope?.Dispose();
        base.Dispose(disposing);
    }

    private IServiceScope RequestScope()
    {
        if (!_reuseScope)
        {
            _scope.Dispose();
            _scope = Services.CreateAsyncScope();
        }
        return _scope;
    }

    public Fixtures Fixtures
    => RequestScope().ServiceProvider.GetRequiredService<Fixtures>();

    public IUserAuthorizationService AuthorizationService => RequestScope().ServiceProvider.GetRequiredService<IUserAuthorizationService>();
}
