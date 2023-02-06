using FakeItEasy;
using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;

namespace PurchaseBuddy.Tests.auth;
internal class RegisterUserTests : AuthorizationTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userRepository = new InMemoryUserRepository();
		authorizationService = new AuthorizationService(userRepository, Configuration);
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
	private IConfiguration configuration;
	private AuthorizationService authorizationService;
}
