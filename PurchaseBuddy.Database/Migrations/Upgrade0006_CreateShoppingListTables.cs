using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(6, "Create shopping list table")]
public class Upgrade0006_CreateShoppingListTables : Migration
{
	public override void Down()
	{
		Delete.Table(ShoppingListTableName);
	}

	public override void Up()
	{
		Create.Table(ShoppingListTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("user_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().ForeignKey("users", "guid")
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("created_at").AsDateTime().NotNullable()
			.WithColumn("completed_at").AsDateTime().Nullable()
			.WithColumn("shop_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).Nullable().ForeignKey("shops", "guid").Nullable()
			.WithColumn("items").AsString().NotNullable();
	}

	private const string ShoppingListTableName = "shopping_lists";
}