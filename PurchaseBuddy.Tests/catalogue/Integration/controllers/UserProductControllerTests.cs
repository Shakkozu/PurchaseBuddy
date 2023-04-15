using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.catalogue.contract;
using System.Net.Http.Json;
using System.Text;

namespace PurchaseBuddy.Tests.catalogue.Integration.controllers;

internal class UserProductControllerTests : ControllersTestsFixture
{
	[Test]
	public async Task UserCategoriesAreReturnesCorrectly()
	{
		var products = await GetProducts();
		Assert.IsEmpty(products);
	}

	private async Task<List<UserProductDto>> GetProducts()
	{
		var response = await httpClient.GetAsync("products");
		if (!response.IsSuccessStatusCode)
			throw new InvalidOperationException();

		return (await response.Content.ReadFromJsonAsync<List<UserProductDto>>()) ?? new List<UserProductDto>();
	}

	private async Task AddNewCategory()
	{
		var body = System.Text.Json.JsonSerializer.Serialize(AUserProductCategoryCreateRequest());
		var response = await httpClient.PostAsync("categories", new StringContent(body, Encoding.UTF8, "application/json"));
		response.EnsureSuccessStatusCode();

	}
	protected CreateUserCategoryRequest AUserProductCategoryCreateRequest(string? name = null)
	{
		return new CreateUserCategoryRequest(name ?? "dairy", null, null);
	}
}
