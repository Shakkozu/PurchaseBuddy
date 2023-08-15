using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(8, "Change datetimes column type to timestamp with time zone in shopping list table")]
public class Upgrade0008_AddInformationAboutTimeZoneToDatesStoredInShoppingTable : Migration
{
	public override void Down()
	{
	}

	public override void Up()
	{
		Execute.Sql(@"ALTER TABLE shopping_lists ADD COLUMN created_at_utc timestamp with time zone;");
		Execute.Sql(@"ALTER TABLE shopping_lists ADD COLUMN completed_at_utc timestamp with time zone;");

		Execute.Sql(@"UPDATE shopping_lists SET created_at_utc = created_at AT TIME ZONE 'UTC';");
		Execute.Sql(@"UPDATE shopping_lists SET completed_at_utc = completed_at AT TIME ZONE 'UTC';");

		Delete.Column("created_at").FromTable(ShoppingListTableName);
		Delete.Column("completed_at").FromTable(ShoppingListTableName);

		Execute.Sql(@"ALTER TABLE shopping_lists RENAME COLUMN created_at_utc TO created_at;");
		Execute.Sql(@"ALTER TABLE shopping_lists RENAME COLUMN completed_at_utc TO completed_at;");
	}

	private const string ShoppingListTableName = "shopping_lists";
}