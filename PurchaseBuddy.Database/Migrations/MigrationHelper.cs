namespace PurchaseBuddy.Database.Migrations;

//[Migration(6, "Create shopping list table")]
//public class Upgrade0006_CreateShoppingListTables : Migration
//{

//	public override void Down()
//	{
//		Delete.Table(ShoppingListTableName);
//	}

//	public override void Up()
//	{
//		Create.Table(ShoppingListTableName)
//			.WithColumn("id").AsGuid().NotNullable().PrimaryKey().Identity()
//			.WithColumn("user_guid").AsGuid().NotNullable().ForeignKey("users", "guid")
//			.WithColumn("guid").AsGuid().NotNullable().Unique()
//			.WithColumn("shop_guid").AsGuid().Nullable().ForeignKey("shops", "guid").Nullable()
//			.WithColumn("items").AsString().Nullable();
//	}

//	private const string ShoppingListTableName = "shopping_lists";
//}

public static class MigrationHelper
{
	public const int GuidColumnLength = 36;
}