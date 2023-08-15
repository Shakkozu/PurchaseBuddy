namespace PurchaseBuddy.API;

public static class LoggerInitializer
{
	public static ILogger<T> InitializeLogger<T>()
	{
		var host = CreateHostBuilder().Build();
		var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

		return loggerFactory.CreateLogger<T>();
	}

	private static IHostBuilder CreateHostBuilder() =>
		Host.CreateDefaultBuilder()
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();
				// TODO: add db
			});
}