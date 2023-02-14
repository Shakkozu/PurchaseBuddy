using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.model;
using PurchaseBuddyLibrary.src.auth.persistance;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PurchaseBuddyLibrary.src.auth.app;
public class AuthorizationService : IAuthorizationService
{
	public AuthorizationService(IUserRepository userRepository,
		IConfiguration configuration
		)
	{
		this.userRepository = userRepository;
		this.configuration = configuration;
	}

	public Guid Register(UserDto userDto)
	{
		if (userRepository.GetByEmail(userDto.Email) != null)
			throw new ArgumentException("User with this email already exists");
		if (userRepository.GetByLogin(userDto.Login) != null)
			throw new ArgumentException("User with this login already exists");

		if (!ValidateIfEmailIsCorrect(userDto.Email))
			throw new ArgumentException("Email is incorrect");

		if (!ValidateIfPasswordIsCorrect(userDto.Password))
			throw new ArgumentException("Password is incorrect");

		var salt = GetHash(Guid.NewGuid().ToString("N"));
		var passwordHash = GetHash(userDto.Password, salt);
		
		var user = User.CreateNew(userDto.Login, userDto.Email, passwordHash, salt);
		userRepository.Add(user);

		return user.Guid;
	}

	public Guid Login(string login, string password)
	{
		var user = userRepository.GetByLogin(login);
		if (user == null)
			throw new ArgumentException("User not found");

		if (user.PasswordHash != GetHash(password + user.Salt))
			throw new ArgumentException("Invalid user credentials");

		var userSession = StaticUserSessionCache.FindByUserId(user.Guid);
		if (userSession != null && !userSession.IsExpired)
			return userSession.SessionId;
		
		var ttlInMinutes = Convert.ToInt32(configuration.GetSection("Authentication")["UserSessionLifetimeInMinutes"]);
		var session = Session.CreateNew(user.Guid, ttlInMinutes);
		StaticUserSessionCache.Add(session);

		return session.SessionId;
	}
	
	public void Logout(Guid sessionId)
	{
		var session = StaticUserSessionCache.Load(sessionId);
		if(session != null)
			StaticUserSessionCache.Delete(session);
	}

	public User GetUserFromSessionId(Guid sessionId)
	{
		var session = StaticUserSessionCache.Load(sessionId);
		
		if (session == null)
			throw new SessionExpiredException(sessionId);

		return userRepository.GetByGuid(session.UserId);
	}

	public UserSessionInfo GetUserSessionInfo(Guid sessionId)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Authentication, sessionId.ToString()),
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var sessionTTLInMinutes = int.Parse(configuration.GetSection("Authentication")["UserSessionLifetimeInMinutes"]);
		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,

			ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(sessionTTLInMinutes),
			// The time at which the authentication ticket expires. A 
			// value set here overrides the ExpireTimeSpan option of 
			// CookieAuthenticationOptions set with AddCookie.

			IsPersistent = false,
			// Whether the authentication session is persisted across 
			// multiple requests. When used with cookies, controls
			// whether the cookie's lifetime is absolute (matching the
			// lifetime of the authentication ticket) or session-based.

			IssuedUtc = DateTimeOffset.UtcNow,
			//IssuedUtc = <DateTimeOffset>,
			// The time at which the authentication ticket was issued.
			RedirectUri = "/login",
			//RedirectUri = <string>
			// The full path or absolute URI to be used as an http 
			// redirect response value.
		};

			//"CustomHeaderAuthentication",
		return new UserSessionInfo(
			CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(claimsIdentity),
			authProperties);
	}

	private bool ValidateIfPasswordIsCorrect(string password)
	{
		return password != null
			&& password.Length >= 6
			&& password.Any(char.IsDigit)
			&& password.Any(char.IsLetter)
			&& password.Any(char.IsUpper)
			&& password.Any(char.IsLower);
	}

	private bool ValidateIfEmailIsCorrect(string email)
	{
		return new EmailAddressAttribute().IsValid(email);
	}

	private string GetHash(string input, string salt = "")
	{
		var salted = input + salt;
		var bytes = Encoding.UTF8.GetBytes(salted);
		var hashBytes = SHA256.HashData(bytes);
		var hash = Convert.ToBase64String(hashBytes);

		return hash;
	}

	private readonly IUserRepository userRepository;
	private readonly IConfiguration configuration;
}