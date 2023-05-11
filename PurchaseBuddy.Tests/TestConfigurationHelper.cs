using Microsoft.Extensions.Configuration;

namespace PurchaseBuddy.Tests;
internal static class TestConfigurationHelper
{
	public static string GetConnectionString()
	{
		var configBuilder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		var configuration = configBuilder.Build();
		var connectionString = configuration.GetConnectionString("Database");
		if(string.IsNullOrEmpty(connectionString))
			throw new ArgumentNullException(nameof(connectionString));

		return connectionString;
	}
}
