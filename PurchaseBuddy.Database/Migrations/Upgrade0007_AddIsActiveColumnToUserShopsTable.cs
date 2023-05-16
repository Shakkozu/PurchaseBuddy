using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(7, "Add is_active column to user_shops table")]
public class Upgrade0007_AddIsActiveColumnToUserShopsTable : Migration
{
    public override void Down()
    {
		Delete.Column("is_active").FromTable(ShopsTableName);
    }

    public override void Up()
    {
		Alter.Table(ShopsTableName).AddColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);
    }

	private const string ShopsTableName = "shops";
}
