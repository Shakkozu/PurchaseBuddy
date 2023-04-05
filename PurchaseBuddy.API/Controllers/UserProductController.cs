using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.catalogue.App.Queries;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("products")]
public class UserProductController : BaseController
{
	public UserProductController(UserProductsManagementService productsManagementService,
		ILogger<UserProductController> logger,
		IUserAuthorizationService authorizationService)
		: base(authorizationService)
	{
		this.productsManagementService = productsManagementService;
		this.logger = logger;
	}

	[HttpPut("{productId}/assign-to/{categoryId}")]
	public async Task<IActionResult> AssignProductTo(Guid? productId, Guid? categoryId)
	{
		var user = await GetUserFromSessionAsync();
		if (productId.HasValue == false || categoryId.HasValue == false)
			return BadRequest("Product id is not provided or category id is not provided.");

		try
		{
			productsManagementService.AssignProductToCategory(user.Guid, productId.Value, categoryId.Value);
		}
		catch (Exception e)
		{
			logger.LogError($"Assigning product {productId} for user {user.Guid} to category {categoryId} failed with error: {e}");
			return BadRequest();
		}

		return NoContent();
	}
	
	[HttpGet]
	public async Task<IActionResult> GetUserProducts(string? filter, int? page, int? pageSize)
	{
		var user = await GetUserFromSessionAsync();
		var query = new GetUserProductsQuery(user.Guid, filter, page ?? 1, pageSize ?? 10000);
		var result = productsManagementService.GetUserProducts(query);

		return Ok(result);
	}
	
	[HttpGet("with-category/{categoryId}")]
	public async Task<IActionResult> GetUserProductsInCategory(Guid categoryId)
	{
		var user = await GetUserFromSessionAsync();
		var query = new GetUserProductsInCategoryQuery(user.Guid, categoryId);
		var result = productsManagementService.GetUserProductsInCategory(query);

		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] UserProductDto request)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			var createdProduct = productsManagementService.DefineNewUserProduct(request, user.Guid);
		}
		catch (Exception e )
		{
			logger.LogError($"Creating product {System.Text.Json.JsonSerializer.Serialize(request)} for user {user.Id} failed with exception: {e}");
			return BadRequest(e.Message);
		}

		return NoContent();
	}
	
	[HttpPut("{productId}")]
	public async Task<IActionResult> Update(Guid productId, [FromBody] UserProductDto request)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			productsManagementService.Modify(productId, request, user.Guid);
		}
		catch (Exception e )
		{
			logger.LogError($"Creating product {System.Text.Json.JsonSerializer.Serialize(request)} for user {user.Id} failed with exception: {e}");
			return BadRequest(e.Message);
		}

		return NoContent();
	}

	private readonly UserProductsManagementService productsManagementService;
	private readonly ILogger<UserProductController> logger;
}