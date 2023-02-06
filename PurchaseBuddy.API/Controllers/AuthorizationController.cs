using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddyLibrary.src.auth.contract;
using System.Security.Claims;
using IAuthorizationService = PurchaseBuddyLibrary.src.auth.app.IAuthorizationService;
using PurchaseBuddyLibrary.src.auth.model;
using PurchaseBuddyLibrary.src.auth.persistance;
using static System.Net.Mime.MediaTypeNames;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
	private readonly IAuthorizationService authorizationService;
	private readonly ILogger<AuthorizationController> logger;
	private readonly IUserRepository userRepository;

	public AuthorizationController(
		IAuthorizationService registerService,
		ILogger<AuthorizationController> logger,
		IUserRepository userRepository)
	{
		this.authorizationService = registerService;
		this.logger = logger;
		this.userRepository = userRepository;
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetLoggedUsername()
	{
		if(!User.HasClaim(claim => claim.Type == ClaimTypes.Authentication))
			await HttpContext.SignOutAsync();

		var sessionId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.Authentication).Value);
		var user = authorizationService.GetUserFromSessionId(sessionId);
		logger.LogInformation($"Cookie value: {Request.Cookies["auth"]}");
		logger.LogInformation($"user cookies: {Request.Cookies}");

		return Ok($"Hello {user.Email}");
	}

	[HttpPost("register")]
	public IActionResult Register([FromBody] UserDto userDto)
	{
		try
		{
			authorizationService.Register(userDto);
			logger.LogInformation($"[AuthorizationController] Registered User {userDto.Email}");

			return Ok();
		}
		catch (Exception e)
		{
			logger.LogError($"[AuthorizationController] Register user request failed with error: {e}");
			return BadRequest(e.Message);
		}
	}
	
	[HttpPost("login")]
	public async Task<dynamic> LoginAsync([FromBody] UserLoginRequest userLoginRequest)
	{
		try
		{
			var sessionId = authorizationService.Login(userLoginRequest.Login, userLoginRequest.Password);
			logger.LogInformation($"[AuthorizationController] Logged user {userLoginRequest.Login}. Session id: {sessionId}");

			var user = userRepository.GetByLogin(userLoginRequest.Login);
			if (user is null)
				throw new Exception();
			
			var sessionContext = authorizationService.GetUserSessionInfo(sessionId);
			
			//await HttpContext.SignInAsync(
			//	sessionContext.Schema,
			//	sessionContext.ClaimsPrincipal,
			//	sessionContext.AuthenticationProperties);

			return Ok(sessionId);
		}
		catch (Exception e)
		{
			logger.LogError($"[AuthorizationController] Register user request failed with error: {e}");
			return BadRequest(e.Message);
		}
	}
	
	[HttpPost("logout")]
	public async Task<IActionResult> LogoutAsync()
	{
		try
		{
			await HttpContext.SignOutAsync();
			authorizationService.Logout(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.Authentication).Value));
			return Ok();
		}
		catch (Exception e)
		{
			logger.LogError($"[AuthorizationController] Register user request failed with error: {e}");
			return BadRequest(e.Message);
		}
	}
}
