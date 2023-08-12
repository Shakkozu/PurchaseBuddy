using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(10, "Add information about allowed users to shopping list")]
public class Upgrade0010_AddInformaitonAboutAllowedUsersToShoppingList : Migration
{
	public override void Down()
	{
		Delete.Column("allowed_users").FromTable(ShoppingListTableName);
	}

	public override void Up()
	{
		Alter.Table(ShoppingListTableName)
			.AddColumn("allowed_users").AsString().Nullable();
	}

	private const string ShoppingListTableName = "shopping_lists";
}