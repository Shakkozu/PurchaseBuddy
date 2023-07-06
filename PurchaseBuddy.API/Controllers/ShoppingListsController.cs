
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.purchases.app.contract;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("shopping-lists")]
public class ShoppingListsController : BaseController
{
	private readonly IShoppingListWriteService shoppingListService;
	private readonly IShoppingListReadService shoppingListReadService;

	public ShoppingListsController(IUserAuthorizationService authorizationService,
		IShoppingListWriteService shoppingListService,
		IShoppingListReadService shoppingListReadService
		)
		: base(authorizationService)
	{
		this.shoppingListService = shoppingListService;
		this.shoppingListReadService = shoppingListReadService;
	}

	[HttpGet]
	public async Task<IActionResult> GetCurrentShoppingLists()
	{
		var user = await GetUserFromSessionAsync();
		var lists = shoppingListReadService.GetNotClosedShoppingLists(user.Guid);

		return Ok(lists);
	}

	[HttpGet("{listId}")]
	public async Task<IActionResult> GetShoppingList(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		var lists = shoppingListReadService.GetShoppingList(user.Guid, listId);

		return Ok(lists);
	}
	
	[HttpPut("{listId}/list-items/{listItemID}/mark-as-purchased")]
	public async Task<IActionResult> MarkListItemAsPurchased(Guid listId, Guid listItemID)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.MarkListItemAsPurchased(user.Guid, listId, listItemID);

		return NoContent();
	}
	
	[HttpPut("{listId}/list-items/{listItemID}/mark-as-not-purchased")]
	public async Task<IActionResult> MarkListItemAsNotPurchased(Guid listId, Guid listItemID)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.MarkListItemAsNotPurchased(user.Guid, listId, listItemID);

		return NoContent();
	}

	[HttpPost]
	public async Task<IActionResult> CreateNew([FromBody] CreateNewShoppingListRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var shoppingListItems = request.ListItems.Select(guid => ShoppingListItem.CreateNew(guid)).ToList();
		var list = shoppingListService.CreateNewList(user.Guid, shoppingListItems, request.AssignedShop);

		return Ok(list);
	}

	[HttpPost("{listId}/list-items")]
	public async Task<IActionResult> AddNewListItem(Guid listId, [FromBody] AddNewListItemRequest request)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.AddNewListItem(user.Guid, listId, request);

		return Ok();
	}

	[HttpDelete("{listId:guid}/list-items/{listItemId:guid}")]
	public async Task<IActionResult> DeleteListItem(Guid listId, Guid listItemId)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.RemoveItemFromList(user.Guid, listId, listItemId);

		return Ok();
	}
}
public record CreateNewShoppingListRequest
{
	public List<Guid> ListItems { get; set; }
	public Guid? AssignedShop { get; set; }
}
public record UpdateShoppingListItemsRequest
{
	public List<Guid> ListItems { get; set; }
}

