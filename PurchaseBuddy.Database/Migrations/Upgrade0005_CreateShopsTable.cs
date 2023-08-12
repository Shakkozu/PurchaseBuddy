using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(5, "Create shopping list table")]
public class Upgrade0005_CreateShopsTable : Migration
{
	public override void Down()
	{
		Delete.Table(ShopsTableName);
	}

	public override void Up()
	{
		Create.Table(ShopsTableName)
			.WithColumn("id").AsGuid().NotNullable().PrimaryKey().Identity()
			.WithColumn("user_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().ForeignKey("users", "guid")
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("name").AsString().NotNullable()
			.WithColumn("description").AsString().Nullable()
			.WithColumn("street").AsString().Nullable()
			.WithColumn("city").AsString().Nullable()
			.WithColumn("local_number").AsString().Nullable()
			.WithColumn("configuration").AsString().Nullable();
	}

	private const string ShopsTableName = "shops";
}