using Microsoft.Extensions.Configuration;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddy.Tests;
internal static class TestConfigurationHelper
{
	private const string elephantURLKey = "ElephantSQLConnectionURL";

	public static string GetConnectionString()
	{
		var urlValue = GetValueFromEnv(elephantURLKey) ?? GetValueFromConfig(elephantURLKey);
		if (string.IsNullOrEmpty(urlValue))
			throw new ArgumentNullException(nameof(urlValue));

		return urlValue.ToConnectionString();
	}

	public static string? GetValueFromConfig(string name)
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<PurchaseBuddyTestsFixture>()
			.Build();

		return configuration.GetValue<string>(name);
	}
	
	private static string? GetValueFromEnv(string name)
	{
		return Environment.GetEnvironmentVariable(name);
	}
}
