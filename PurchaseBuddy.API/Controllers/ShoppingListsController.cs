
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.purchases.domain;
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
	
	[HttpPut("{listId}/products/{productId}/mark-as-purchased")]
	public async Task<IActionResult> MarkListItemAsPurchased(Guid listId, Guid productId)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.MarkProductAsPurchased(user.Guid, listId, productId);

		return NoContent();
	}
	
	[HttpPatch("{listId}/products")]
	public async Task<IActionResult> UpdateProducts(Guid listId, [FromBody] UpdateShoppingListItemsRequest request)
	{
		var user = await GetUserFromSessionAsync();
		shoppingListService.UpdateProductsOnList(user.Guid, listId, request.ListItems);

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
		var shoppingListItems = request.ListItems.Select(guid => ShoppingListItem.CreateNew(guid)).ToList();
		var list = shoppingListService.CreateNewList(user.Guid, shoppingListItems, request.AssignedShop);

		return Ok(list);
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

