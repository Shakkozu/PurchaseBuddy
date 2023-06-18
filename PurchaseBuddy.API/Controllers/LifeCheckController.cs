using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PurchaseBuddy.API.Controllers;
public class LifeCheckController : Controller
{
	public IActionResult Index()
	{
		return Ok("Im alive");
	}

	[HttpGet("/health-check")]
	public IActionResult HealthCheck()
	{
		return Ok("Im alive");
	}
}
