using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.Tests;
internal static class TestConfigurationHelper
{
	private const string elephantURLKey = "ElephantSQLConnectionURL";

	public static string GetConnectionString()
	{
		var urlValue = GetValueFromEnv(elephantURLKey);
		if (string.IsNullOrEmpty(urlValue))
			return GetConnectionString("Database");

		return urlValue.ToConnectionString();
	}

	public static string? GetValueFromConfig(string name)
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appSettings.json", optional: true, true)
			.AddUserSecrets<PurchaseBuddyTestsFixture>()
			.Build();

		return configuration.GetValue<string>(name);
	}
	
	private static string? GetConnectionString(string name)
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appSettings.json", optional: true, true)
			.AddUserSecrets<PurchaseBuddyTestsFixture>()
			.Build();

		return configuration.GetConnectionString(name);
	}
	
	private static string? GetValueFromEnv(string name)
	{
		return Environment.GetEnvironmentVariable(name);
	}
}
