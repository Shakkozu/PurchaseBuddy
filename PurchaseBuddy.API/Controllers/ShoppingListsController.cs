
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.purchases.app;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.src.auth.app;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("shopping-lists")]
public class ShoppingListsController : BaseController
{
	private readonly IShoppingListService shoppingListService;

	public ShoppingListsController(IUserAuthorizationService authorizationService,
		IShoppingListService shoppingListService
		)
		: base(authorizationService)
	{
		this.shoppingListService = shoppingListService;
	}

	[HttpGet]
	public async Task<IActionResult> GetCurrentShoppingLists()
	{
		var user = await GetUserFromSessionAsync();
		var lists = shoppingListService.GetNotClosedShoppingLists(user.Guid);

		return Ok(lists);
	}

	[HttpGet("{listId}")]
	public async Task<IActionResult> GetShoppingList(Guid listId)
	{
		var user = await GetUserFromSessionAsync();
		var lists = shoppingListService.GetShoppingList(user.Guid, listId);

		return Ok(lists);
	}
	
	[HttpPut("{listId}/products/{productId}/mark-as-purchased")]
	public async Task<IActionResult> MarkListItemAsPurchased(Guid listId, Guid productId)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.MarkProductAsPurchased(user.Guid, listId, productId);

		return NoContent();
	}
	
	[HttpPut("{listId}/products/{productId}/mark-as-not-purchased")]
	public async Task<IActionResult> MarkListItemAsNotPurchased(Guid listId, Guid productId)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.MarkProductAsNotPurchased(user.Guid, listId, productId);

		return NoContent();
	}

	[HttpPost]
	public async Task<IActionResult> CreateNew([FromBody] CreateNewShoppingListRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var shoppingListItems = request.ListItems.Select(guid => new ShoppingListItem(guid)).ToList();
		var list = shoppingListService.CreateNewList(user.Guid, shoppingListItems, request.AssignedShop);

		return Ok(list);
	}
}
public record CreateNewShoppingListRequest
{
	public List<Guid> ListItems { get; set; }
	public Guid? AssignedShop { get; set; }
}

