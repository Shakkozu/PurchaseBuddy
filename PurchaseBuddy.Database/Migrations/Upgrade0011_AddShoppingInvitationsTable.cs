using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(11, "Add shopping invitations table")]
public class Upgrade0011_AddShoppingInvitationsTable : Migration
{
	public override void Down()
	{
		Delete.Table(ShoppingInvitationsTableName);
	}

	public override void Up()
	{
		Create.Table(ShoppingInvitationsTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable()
			.WithColumn("shopping_creator_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().ForeignKey("users", "guid")
			.WithColumn("shopping_list_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().ForeignKey("shopping_lists", "guid")
			.WithColumn("is_active").AsBoolean().NotNullable()
			.WithColumn("invited_users").AsString().NotNullable()
			.WithColumn("allowed_users").AsString().NotNullable();
	}

	private const string ShoppingInvitationsTableName = "shopping_invitations";
}