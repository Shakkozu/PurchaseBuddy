namespace PurchaseBuddy.API;

using PurchaseBuddyLibrary.src.utils;

public class ConnectionStringProvider
{
	public static string? GetConnectionString(IConfiguration configuration)
	{
		string connectionString;
		string environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
		var logger = LoggerInitializer.InitializeLogger<ConnectionStringProvider>();
		logger.LogInformation($"[ConnectionStringProvider] Retrieved '{environment}' value from 'ASPNETCORE_ENVIRONMENT' variable");

		if (environment == "Development")
			return configuration.GetConnectionString("Database");

		return configuration.GetValue<string>("ElephantSQLConnectionURL").ToConnectionString();
	}
}
