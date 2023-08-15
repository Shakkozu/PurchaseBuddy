using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.persistance;
using System.Net;
using System.Security.Claims;

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
    public IActionResult Register([FromBody] RegisterUserRequest userDto)
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

			var response = new LoginResponse(sessionId, user.Guid);
			return Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError($"[AuthorizationController] Register user request failed with error: {e}");

            return BadRequest(e.Message);
        }
    }

    [HttpPost("logout")]
    public IActionResult LogoutAsync()
    {
        try
        {
			var userAuthenticationClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Authentication);
			if(userAuthenticationClaim != null)
				authorizationService.Logout(Guid.Parse(userAuthenticationClaim.Value));
        }

        catch (Exception e)
        {
            logger.LogError($"[AuthorizationController] Register user request failed with error: {e}");
            return BadRequest(e.Message);
        }
        return Ok();
    }
}

public record LoginResponse(Guid SessionId, Guid UserId);

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
