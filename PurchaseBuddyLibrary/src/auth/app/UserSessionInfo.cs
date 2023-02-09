using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PurchaseBuddyLibrary.src.auth.app;
public class UserSessionInfo
{
	public UserSessionInfo(string schema, ClaimsPrincipal claimsPrincipal, AuthenticationProperties authenticationProperties)
	{
		Schema = schema;
		ClaimsPrincipal = claimsPrincipal;
		AuthenticationProperties = authenticationProperties;
	}
	public string Schema { get; }
	public ClaimsPrincipal ClaimsPrincipal { get;  }
	public AuthenticationProperties AuthenticationProperties { get; }
}
