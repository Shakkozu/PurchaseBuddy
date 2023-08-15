
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.purchases.CloningListsToOtherUsers;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("shared-shopping-lists")]
public class SharingShoppingListsController : BaseController
{
	private readonly ShoppingListSharingFacade facade;

	public SharingShoppingListsController(IUserAuthorizationService authorizationService,
		ShoppingListSharingFacade shoppingListSharingFacade
		)
		: base(authorizationService)
	{
		this.facade = shoppingListSharingFacade;
	}

	[HttpGet("{listId}")]
	public IActionResult GetSharedList(Guid listId)
	{
		var list = facade.GetSharedList(listId);

        return Ok(new
		{
			list.Items,
			list.CreatedAt,
			list.CreatorId,
			list.Guid
		});
	}
	
	[HttpPut("{listId}/share")]
	public async Task<IActionResult> CreateSharedShoppingList(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		var list = facade.CreateSharedList(user.Guid, listId);

		return Ok(list);
	}
	
	[HttpPut("{listId}/import")]
	public async Task<IActionResult> ImportList(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		var importedListId = facade.ImportSharedList(user.Guid, listId);

		return Ok(importedListId);
	}
}

