using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using PurchaseBuddy.API.Controllers;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddy.Tests.auth;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.stores.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseBuddy.Tests.stores.integration;

internal class UserShopControllerTests : AuthorizationTestsFixture
{
	private InMemoryUserShopRepository userShopRepository;
	private UserShopService userShopService;
	private UserShopController userShopController;
	private IUserAuthorizationService authorizationService;
	private HttpClient client;

	[SetUp]
	public void SetUp()
	{
		userShopRepository = new InMemoryUserShopRepository();
		userShopService = new UserShopService(userShopRepository);
		authorizationService = new AuthorizationService(new InMemoryUserRepository(), Configuration);
		userShopController = new UserShopController(userShopService, A.Fake<ILogger<UserShopController>>(), authorizationService);
	}

	private Guid CreateAndLogUser()
	{
		authorizationService.Register(AUser());
		return authorizationService.Login(AUser().Login, AUser().Password);
	}

	[Test]
	public async Task TestGetShops()
	{
		CreateAndLogUser();	
		var shops = await userShopController.GetUserShops();
		var a = 1;
		Assert.NotNull(shops);
	}

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		var builder = new WebHostBuilder()
		   .UseEnvironment("Development")
		   .UseUrls("http://localhost:5000")
		   .UseStartup<Program>();
		var server = new TestServer(builder);
		client = server.CreateClient();

		var sessionId = CreateAndLogUser();
		client.DefaultRequestHeaders.Add("Authorization", sessionId.ToString());
	}

	[Test]
	public async Task TestGetShops2()
	{
		var shops = await client.GetFromJsonAsync<List<UserShopDto>>("shops");
		var a = 1;
		Assert.NotNull(shops);
	}
}
