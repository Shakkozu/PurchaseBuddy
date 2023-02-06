using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurchaseBuddyLibrary.src.auth.app;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class CustomHeaderAuthenticationHandler : AuthenticationHandler<CustomHeaderAuthenticationOptions>
{

	public CustomHeaderAuthenticationHandler(
		IOptionsMonitor<CustomHeaderAuthenticationOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
		: base(options, logger, encoder, clock)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValues))
		{
			return Task.FromResult(AuthenticateResult.Fail("Missing authentication header."));
		}

		var headerValue = headerValues.FirstOrDefault();

		if (string.IsNullOrEmpty(headerValue) || StaticUserSessionCache.Load(Guid.Parse(headerValue)) == null)
		{
			return Task.FromResult(AuthenticateResult.Fail("Invalid authentication header value."));
		}

		var claims = new[] { new Claim(ClaimTypes.Name, headerValue) };
		var identity = new ClaimsIdentity(claims, Options.HeaderName);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Options.HeaderName);

		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}

public class CustomHeaderAuthenticationOptions : AuthenticationSchemeOptions
{
	public string HeaderName { get; set; }
	public string HeaderValue { get; set; }
}