using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PurchaseBuddy.API.Controllers;
public class LifeCheckController : Controller
{
	private readonly ILogger<LifeCheckController> logger;

	public LifeCheckController(ILogger<LifeCheckController> logger)
    {
		this.logger = logger;
	}

    [HttpGet("/health-check")]
	public IActionResult HealthCheck()
	{
		var configBuilder = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.AddUserSecrets<Program>();
		var configuration = configBuilder.Build();
		var databaseConnectionString = configuration.GetValue<string>("ElephantSQLConnectionURL");
		if (string.IsNullOrWhiteSpace(databaseConnectionString))
            logger.LogError("Retrieving value from configuration failed");

		return Ok("Im alive");
	}
}
