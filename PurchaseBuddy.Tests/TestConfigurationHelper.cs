using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.Tests;
internal static class TestConfigurationHelper
{
	public static string GetConnectionString()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<PurchaseBuddyTestsFixture>()
			.Build();
		var connectionString = configuration.GetValue<string>("ElephantSQLConnectionURL").ToConnectionString();
		if(string.IsNullOrEmpty(connectionString))
			throw new ArgumentNullException(nameof(connectionString));

		return connectionString;
	}
}
