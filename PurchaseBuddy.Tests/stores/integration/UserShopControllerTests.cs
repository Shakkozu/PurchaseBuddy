using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using PurchaseBuddyLibrary.src.stores.contract;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PurchaseBuddy.Tests.stores.integration;
internal class UserShopControllerTests : ControllersTestsFixture
{
	[Test]
	public async Task TestGetShops()
	{
		var response = await httpClient.GetAsync("shops");

		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	}

	[Test]
	public void WhenUserDidNotAddAnyShop_ReturnEmptyList()
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
		DeleteUserShop(userShops[0].Guid.Value);
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
