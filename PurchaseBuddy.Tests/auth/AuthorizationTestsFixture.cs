using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.auth.contract;

namespace PurchaseBuddy.Tests.auth;

[TestFixture]
internal abstract class AuthorizationTestsFixture
{
    [SetUp]
    public virtual void SetUp()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        Configuration = builder.Build();
    }

    protected IConfiguration Configuration { get; private set; }

    protected RegisterUserRequest AUser()
    {
        return new RegisterUserRequest
        {
            Email = "john.doe@example.com",
            Login = "johnDoe",
            Password = "zaq1@WSX"
        };
    }
}
