using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using PurchaseBuddy.Database.Migrations;
using System.Linq;
using System;

namespace PurchaseBuddy.Database;
public static class MigrationsRunner
{
	public static void RunMigrations(IServiceCollection serviceCollection, string connectionString)
	{
		IMigrationRunner migrator;
		var isRegistered = serviceCollection.BuildServiceProvider().GetServices<IMigrationRunner>().Any();
		if (!isRegistered)
		{
			var serviceProvider = serviceCollection.AddFluentMigratorCore()
				.ConfigureRunner(rb =>
					rb.AddPostgres()
					.WithGlobalConnectionString(connectionString)
					.ScanIn(typeof(Upgrade0001_InitializeUsers).Assembly).For.Migrations())
				.BuildServiceProvider();

			migrator = serviceProvider.GetRequiredService<IMigrationRunner>();
		}
		else
			migrator = serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>();
		migrator.MigrateUp();
	}
	
	public static void ClearDatabase(ServiceCollection serviceCollection, string connectionString)
	{

		var sc = new ServiceCollection();
		//serviceCollection.IsRegistered()
		//var migrator = serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>();
		IMigrationRunner migrator;
		var isRegistered = serviceCollection.BuildServiceProvider().GetServices<IMigrationRunner>().Any();
		if (!isRegistered)
		{
			var serviceProvider = serviceCollection.AddFluentMigratorCore()
				.ConfigureRunner(rb =>
					rb.AddPostgres()
					.WithGlobalConnectionString(connectionString)
					.ScanIn(typeof(Upgrade0001_InitializeUsers).Assembly).For.Migrations())
				.BuildServiceProvider();

			migrator = serviceProvider.GetRequiredService<IMigrationRunner>();
		}
		else
			migrator = serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>();


		migrator.MigrateDown(0);
	}
}
