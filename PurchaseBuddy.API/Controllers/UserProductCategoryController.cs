using Microsoft.AspNetCore.Mvc;
using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddyLibrary.src.auth.app;
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
		var userCategories = categoriesManagementService.GetUserProductCategories(user.Guid);
		var result = userCategories.Select(category => new ProductCategoryDto(category));


		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateUserCategoryRequest request)
	{
		var user = await GetUserFromSessionAsync();
		var createdCategoryId = categoriesManagementService.AddNewProductCategory(user.Guid, request);

		return Ok(createdCategoryId.ToString());
	}

	private readonly UserProductCategoriesManagementService categoriesManagementService;
	private readonly ILogger<UserProductCategoryController> logger;
}
