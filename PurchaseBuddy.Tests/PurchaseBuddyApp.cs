using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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

    public static PurchaseBuddyApp CreateInstance()
    {
        var app = new PurchaseBuddyApp(_ => { });
        return app;
    }

    public static PurchaseBuddyApp CreateInstance(Action<IServiceCollection> customization)
    {
        var purchaseBuddyApp = new PurchaseBuddyApp(customization);
        return purchaseBuddyApp;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(collection => collection.AddTransient<Fixtures>());
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
