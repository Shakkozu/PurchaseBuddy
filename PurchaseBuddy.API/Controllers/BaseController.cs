using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IUserAuthorizationService = PurchaseBuddyLibrary.src.auth.app.IUserAuthorizationService;
using PurchaseBuddyLibrary.src.auth.model;

namespace PurchaseBuddy.API.Controllers;

public abstract class BaseController : ControllerBase
{
	private readonly IUserAuthorizationService authorizationService;

	public BaseController(IUserAuthorizationService authorizationService)
	{
		this.authorizationService = authorizationService;
	}

	protected async Task<User> GetUserFromSessionAsync()
	{
		if (!User.HasClaim(claim => claim.Type == ClaimTypes.Authentication))
			await HttpContext.SignOutAsync();

		var sessionId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.Authentication).Value);
		var user = authorizationService.GetUserFromSessionId(sessionId);
		if (user == null)
		{
			await HttpContext.SignOutAsync();
			throw new ArgumentException("User not found exception");
		}

		return user;
	}
}
