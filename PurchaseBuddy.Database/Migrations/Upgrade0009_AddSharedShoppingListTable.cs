using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(9, "Add shared shopping list table")]
public class Upgrade0009_AddSharedShoppingListTable : Migration
{
	public override void Down()
	{
		Delete.Table(SharedShoppingListTableName);
	}

	public override void Up()
	{
		Create.Table(SharedShoppingListTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("creator_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().ForeignKey("users", "guid")
			.WithColumn("source_list_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable()
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("items").AsString().NotNullable();
	}

	private const string SharedShoppingListTableName = "shared_shopping_lists";
}
