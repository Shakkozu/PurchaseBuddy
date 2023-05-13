using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(1, "Initialize users table")]
public class Upgrade0001_InitializeUsers : Migration
{
	public override void Down()
	{
		Delete.Table("users");
	}

	public override void Up()
	{
		Create.Table("users")
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(36).NotNullable().Unique()
			.WithColumn("email").AsString(255).NotNullable().Unique()
			.WithColumn("login").AsString(255).NotNullable()
			.WithColumn("salt").AsString().NotNullable()
			.WithColumn("is_administrator").AsBoolean().Nullable()
			.WithColumn("password_hash").AsString().NotNullable();
	}
}
