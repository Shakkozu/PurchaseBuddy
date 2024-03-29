﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.stores.app;
using PurchaseBuddy.src.stores.domain;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.stores.contract;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("shops")]
public class UserShopController : BaseController
{
	public UserShopController(UserShopService userShopService,
		ILogger<UserShopController> logger,
		IUserAuthorizationService authorizationService)
		: base(authorizationService)
	{
		this.userShopService = userShopService;
	}

	[HttpGet]
	public async Task<IActionResult> GetUserShops()
	{
		var user = await GetUserFromSessionAsync();
		var userShops = userShopService.GetAllUserShops(user.Guid);
		var result = userShops.Select(UserShopDto.FromModel);

		return Ok(result);
	}

	[HttpGet("{shopId}")]
	public async Task<IActionResult> GetUserShop(Guid shopId)
	{
		var user = await GetUserFromSessionAsync();
		var userShop = userShopService.GetUserShopById(user.Guid, shopId);
		if (userShop is null)
			return NotFound();

		var result = UserShopDto.FromModel(userShop);
		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] UserShopDto userShopDto)
	{
		var user = await GetUserFromSessionAsync();
		var address = Address.From(userShopDto.Address);
		var userShopDescription = UserShopDescription.CreateNew(userShopDto.Name, userShopDto.Description, address);
		userShopService.AddNew(user.Guid, userShopDescription, userShopDto.CategoriesMap);

		return NoContent();
	}

	[HttpPut("{shopId}")]
	public async Task<IActionResult> Update([FromBody] UserShopDto userShopDto, Guid shopId)
	{
		var user = await GetUserFromSessionAsync();
		var address = Address.From(userShopDto.Address);
		var userShopDescription = UserShopDescription.CreateNew(userShopDto.Name, userShopDto.Description, address);
		userShopService.Update(userShopDescription, user.Guid, shopId, userShopDto.CategoriesMap);

		return NoContent();
	}

	[HttpDelete("{shopId}")]
	public async Task<IActionResult> Delete(Guid shopId)
	{
		var user = await GetUserFromSessionAsync();
		userShopService.DeleteUserShop(user.Guid, shopId);

		return Ok();
	}

	private readonly UserShopService userShopService;
}

