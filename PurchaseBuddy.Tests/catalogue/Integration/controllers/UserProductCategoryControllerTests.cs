using Newtonsoft.Json;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.Tests.stores.integration;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PurchaseBuddy.Tests.catalogue.Integration.controllers;
internal class UserProductCategoryControllerTests : ControllersTestsFixture
{
	[Test]
	public async Task UserCanAddCategory()
	{
		await AddNewCategory();
		var categories = await GetProductCategories();
		Assert.AreEqual(1, categories.Count);

		await AddNewCategory();
		await AddNewCategory();
		categories = await GetProductCategories();
		Assert.AreEqual(3, categories.Count);
	}

	private async Task<List<ProductCategoryDto>> GetProductCategories()
	{
		var response = await httpClient.GetAsync("categories");
		if (!response.IsSuccessStatusCode)
			throw new InvalidOperationException();

		return await response.Content.ReadFromJsonAsync<List<ProductCategoryDto>>();
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
