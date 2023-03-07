using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.API.Controllers;

[ApiController]
// to test purposes
//[Authorize]
[Route("categories")]
public class UserProductCategoryController : BaseController
{
	public UserProductCategoryController(UserProductCategoriesManagementService categoriesManagementService,
		ILogger<UserProductCategoryController> logger,
		IUserAuthorizationService authorizationService)
		: base(authorizationService)
	{
		this.categoriesManagementService = categoriesManagementService;
		this.logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetUserCategories()
	{
		var user = await GetUserFromSessionAsync();
		var result = categoriesManagementService.GetProductCategories2(user.Guid);

		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateUserCategoryRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var createdCategoryId = categoriesManagementService.AddNewProductCategory(user.Guid, request);

		return Ok(createdCategoryId.ToString());
	
	}

	[HttpPost()]
	[Route("reassign/{categoryId}/to/{newParentCategoryId}")]
	public async Task<IActionResult> Create(Guid categoryId, Guid newParentCategoryId)
	{
		var user = await GetUserFromSessionAsync();
		try
		{
			categoriesManagementService.ReassignUserProductCategory(user.Guid, categoryId, newParentCategoryId);
		}
		catch (Exception e )
		{
			logger.LogError($"Reassignment of product category {categoryId} failed for used {user.Id} with exception: {e}");
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
		var c2 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("Category 2", "Category 2 description", null));
		var sc1 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 1", "Subcategory 1 description", c1));
		var sc11 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 12", "Subcategory 2 description", c1));
		var sc12 = categoriesManagementService.AddNewProductCategory(user.Guid, new CreateUserCategoryRequest("SubCategory 11", "Subcategory 11 description", sc11));

		return Ok();
	}

	private readonly UserProductCategoriesManagementService categoriesManagementService;
	private readonly ILogger<UserProductCategoryController> logger;
}
