using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PurchaseBuddyLibrary.src.stores.controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class UserShopController : ControllerBase
{
	[HttpGet]
	public ActionResult<string> Get()
	{
		return Ok("Success!");
	}

}
public class GetUserShopsResponse
{
	public List<UserShopDto> UserShops{ get; set; }
}

public class UserShopDto
{
}