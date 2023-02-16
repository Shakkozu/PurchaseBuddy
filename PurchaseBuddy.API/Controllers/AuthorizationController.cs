using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddyLibrary.src.auth.contract;
using System.Security.Claims;
using System.Net;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.auth.app;

namespace PurchaseBuddy.API.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthorizationController : BaseController
{
	private readonly IUserAuthorizationService authorizationService;
	private readonly ILogger<AuthorizationController> logger;
	private readonly IUserRepository userRepository;

	public AuthorizationController(
		IUserAuthorizationService authorizationService,
		ILogger<AuthorizationController> logger,
		IUserRepository userRepository) : base(authorizationService)
	{
		this.authorizationService = authorizationService;
		this.logger = logger;
		this.userRepository = userRepository;
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetLoggedUsername()
	{
		var user = await GetUserFromSessionAsync();
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
	public dynamic LoginAsync([FromBody] UserLoginRequest userLoginRequest)
	{
		try
		{
			var sessionId = authorizationService.Login(userLoginRequest.Login, userLoginRequest.Password);
			logger.LogInformation($"[AuthorizationController] Logged user {userLoginRequest.Login}. Session id: {sessionId}");

			var user = userRepository.GetByLogin(userLoginRequest.Login);
			if (user is null)
				throw new Exception();
			
			var sessionContext = authorizationService.GetUserSessionInfo(sessionId);
			
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

public record ErrorResponse
{
	public ErrorResponse(string error, HttpStatusCode statusCode)
	{
		ErrorMessage = error;
		StatusCode = statusCode;
	}
	public string ErrorMessage { get; set; }
	public HttpStatusCode StatusCode { get; set; }
}
