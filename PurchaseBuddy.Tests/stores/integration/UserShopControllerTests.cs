using PurchaseBuddy.API.Controllers;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddy.src.stores.persistance;
using PurchaseBuddy.Tests.auth;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.stores.contract;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace PurchaseBuddy.Tests.stores.integration;

internal class UserShopControllerTests
{
	private Guid sessionId;
	private PurchaseBuddyApp app;
	private HttpClient httpClient;
	private Fixtures Fixtures => app.Fixtures;

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

	[Test]
	public async Task TestGetShops()
	{
		var response = await httpClient.GetAsync("shops");

		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	}
	
	[Test]
	public async Task WhenUserDidNotAddAnyShop_ReturnEmptyList()
	{
		// when user did not add any shop
		// then user does not have any shop
		Assert.IsEmpty(GetUserShops());
	}
	
	[Test]
	public void TestUserShopsManagement()
	{
		// when user adds new shop
		AddNewUserShop();
		// then user shops list contains new shop
		var userShops = GetUserShops();
		Assert.AreEqual(1, userShops.Count);
	}
	
	[Test]
	public void TestUserShopModification()
	{
		// when user adds new shop
		AddNewUserShop();
		// then user shops list contains new shop
		var userShops = GetUserShops();
		Assert.AreEqual(1, userShops.Count);
		// when user modifies existing shop
		ModifyUserShop(userShops[0], "testNewName");
		// then shop is modified
		// thenshops count not changed
		userShops = GetUserShops();
		Assert.AreEqual(1, userShops.Count);
		Assert.AreEqual("testNewName", userShops[0].Name);
	}
	
	[Test]
	public void TestUserShopRemoving()
	{
		// when user adds new shop
		AddNewUserShop();
		// then user shops list contains new shop
		var userShops = GetUserShops();
		Assert.AreEqual(1, userShops.Count);
		// when user removes existing shop
		DeleteUserShop(userShops[0].Guid);
		// then user shops list is empty
		userShops = GetUserShops();
		Assert.IsEmpty(userShops);
	}

	private void DeleteUserShop(Guid guid)
	{
		var response = httpClient.DeleteAsync($"shops/{guid:N}").GetAwaiter().GetResult();
	}

	private void ModifyUserShop(UserShopDto userShopDto, string newName)
	{
		userShopDto.Name = newName;
		var body = JsonSerializer.Serialize(userShopDto);
		var response = httpClient.PutAsync($"shops/{userShopDto.Guid:N}", new StringContent(body, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
	}

	private void AddNewUserShop()
	{
		var body = JsonSerializer.Serialize(AUserShop());
		var response = httpClient.PostAsync("shops", new StringContent(body, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
	}

	private UserShopDto AUserShop()
	{
		return new UserShopDto
		{
			Address = new AddressDto
			{
				City = "Poznań",
				LocalNumber = "20",
				Street = "Głogowska"
			},
			Description = "testdesc",
			Name = "Głogowska Biedronka",
			Guid = Guid.NewGuid()
		};
	}

	private List<UserShopDto> GetUserShops()
	{
		var response = httpClient.GetAsync("shops").GetAwaiter().GetResult();
		return response.Content.ReadFromJsonAsync<List<UserShopDto>>().GetAwaiter().GetResult();
	}
}
