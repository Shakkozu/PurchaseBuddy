using FakeItEasy;
using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddy.Tests.auth;

[TestFixture]
internal abstract class AuthorizationTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		var builder = new ConfigurationBuilder();
		builder.AddJsonFile("appsettings.json");
		Configuration = builder.Build();
	}

	protected IConfiguration Configuration { get; private set; }

	protected UserDto AUser()
	{
		return new UserDto
		{
			Email = "john.doe@example.com",
			Login = "johnDoe",
			Password = "zaq1@WSX"
		};
	}
}
internal class LoginServiceTests : AuthorizationTestsFixture
{
	private InMemoryUserRepository userRepository;
	private AuthorizationService authorizationService;
	private UserSessionCache sessionRepository;

	[SetUp]
	public void SetUp()
	{
		userRepository = new InMemoryUserRepository();
		sessionRepository = new UserSessionCache();
		authorizationService = new AuthorizationService(userRepository, sessionRepository, Configuration);
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
internal class RegisterUserTests : AuthorizationTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userRepository = new InMemoryUserRepository();
		sessionRepository = new UserSessionCache();
		authorizationService = new AuthorizationService(userRepository, sessionRepository, Configuration);
	}

	[Test]
	public void RegisterUser_AsserUserWithHashedPasswordIsCreated()
	{
		var user = AUser();
		var registeredUserGuid = authorizationService.Register(user);

		Assert.NotNull(userRepository.GetByGuid(registeredUserGuid));
	}
	
	[Test]
	public void RegisterUser_WhenEmailIsInvalid_AssertRegistrationFailed()
	{
		var user = AUser();
		user.Email= "12";

		Assert.Throws<ArgumentException>(() => authorizationService.Register(user));
	}
	
	[TestCase("11")]
	[TestCase("zaqwsxedc")]
	[TestCase("123456")]
	[TestCase("ioudioi")]
	[TestCase("QQQQQQQQQQ")]
	public void RegisterUser_WhenPasswordIsInvalid_AssertRegistrationFailed(string password)
	{
		var user = AUser();
		user.Password = password;

		Assert.Throws<ArgumentException>(() => authorizationService.Register(user));
	}

	private InMemoryUserRepository userRepository;
	private UserSessionCache sessionRepository;
	private IConfiguration configuration;
	private AuthorizationService authorizationService;
}
