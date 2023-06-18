using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.catalogue.contract;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
[Authorize]
[Route("categories")]
public class UserProductCategoryController : BaseController
{
	public UserProductCategoryController(IUserProductCategoriesManagementService categoriesManagementService,
		ILogger<UserProductCategoryController> logger,
		CategoryFacade productsFacade,
		IUserAuthorizationService authorizationService)
		: base(authorizationService)
	{
		this.categoriesManagementService = categoriesManagementService;
		this.logger = logger;
		this.productsFacade = productsFacade;
	}

	[HttpGet]
	public async Task<IActionResult> GetUserCategories()
	{
		var user = await GetUserFromSessionAsync();
		var result = categoriesManagementService.GetUserProductCategories(user.Guid);

		return Ok(result.Categories);
	}

	[HttpPost]
	public async Task<dynamic> Create([FromBody] CreateUserCategoryRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var createdCategoryId = categoriesManagementService.AddNewProductCategory(user.Guid, request);

		return Ok(createdCategoryId);
	
	}
	
	[HttpPost("remove/{categoryId}/reassign-products-to/{newCategoryId?}")]
	public async Task<dynamic> RemoveAndReassignProducts(Guid categoryId, Guid? newCategoryId)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			productsFacade.RemoveCategoryAndReassignProducts(user.Guid, categoryId, newCategoryId);
			return NoContent();
		}
		catch (Exception e)
		{
			logger.LogError($"Removing product category {categoryId} failed for used {user.Id} with exception: {e}");
			return BadRequest(e.Message);
		}
	}

	[HttpPost()]
	[Route("reassign/{categoryId}/to/{newParentCategoryId?}")]
	public async Task<dynamic> Reassign(Guid categoryId, Guid newParentCategoryId)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			categoriesManagementService.ReassignCategory(user.Guid, categoryId, newParentCategoryId);
		}
		catch (Exception e )
		{
			logger.LogError($"Reassignment of product category {categoryId} failed for used {user.Id} with exception: {e}");
			return BadRequest(e.Message);
		}

		return NoContent();
	}

	[HttpDelete]
	[Route("{categoryId}")]
	public async Task<dynamic> DeleteAsync(Guid categoryId)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			categoriesManagementService.DeleteCategory(user.Guid, categoryId);
		}
		catch (Exception e)
		{
			logger.LogError($"Removing product category {categoryId} failed for used {user.Id} with exception: {e}");
			return BadRequest(e.Message);
		}

		return NoContent();
	}
	
	[HttpPost]
	[Route("seed")]
	public async Task<IActionResult> AddSampleUserCategories()
	{
		var user = await GetUserFromSessionAsync();
		var c1 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("Category 1", "Category 1 description", null));
		categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("Category 2", "Category 2 description", null));
		categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 1", "Subcategory 1 description", c1));
		var sc11 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 12", "Subcategory 2 description", c1));
		categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 11", "Subcategory 11 description", sc11));

		return Ok();
	}

	private readonly IUserProductCategoriesManagementService categoriesManagementService;
	private readonly ILogger<UserProductCategoryController> logger;
	private readonly CategoryFacade productsFacade;
}
