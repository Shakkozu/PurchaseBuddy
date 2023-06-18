using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurchaseBuddyLibrary.src.auth.persistance;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PurchaseBuddyLibrary.src.auth.infra
{
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
			// todo endpoints without Authorize attribute should not be verified with this handler
			if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValues))
			{
				return Task.FromResult(AuthenticateResult.Fail("Missing authentication header."));
			}

			var authorizationHeader = headerValues.FirstOrDefault();
			if (!IsAuthenticated(authorizationHeader))
				return Task.FromResult(AuthenticateResult.Fail("Authorization failed"));

			var claims = new[] { new Claim(ClaimTypes.Authentication, authorizationHeader) };
			var identity = new ClaimsIdentity(claims, Options.HeaderName);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Options.HeaderName);

			return Task.FromResult(AuthenticateResult.Success(ticket));
		}

		private static bool IsAuthenticated(string? sessionId)
		{
			if (string.IsNullOrEmpty(sessionId))
				return false;

			var userSession = StaticUserSessionCache.Load(Guid.Parse(sessionId));

			return userSession != null && !userSession.IsExpired;
		}
	}

	public class CustomHeaderAuthenticationOptions : AuthenticationSchemeOptions
	{
		public string? HeaderName { get; set; }
		public string? HeaderValue { get; set; }
	}
}