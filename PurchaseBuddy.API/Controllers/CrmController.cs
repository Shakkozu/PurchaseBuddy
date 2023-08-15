using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.crm;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CrmController : BaseController
{
	private readonly IUsersProvider usersProvider;

    public CrmController(
        IUserAuthorizationService authorizationService,
        IUsersProvider usersProvider) : base(authorizationService)
    {
		this.usersProvider = usersProvider;
    }

    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var user = await GetUserFromSessionAsync();
		var result = usersProvider.GetAllUsers().Where(x => x.Guid != user.Guid);

		return Ok(result);
    }
}
