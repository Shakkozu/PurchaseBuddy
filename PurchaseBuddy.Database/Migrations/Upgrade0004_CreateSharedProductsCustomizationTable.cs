using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(4, "Create shared products customization table")]
public class Upgrade0004_CreateSharedProductsCustomization: Migration
{
	public override void Down()
	{
		Delete.Table(SharedProductsCustomizationTable);
	}

	public override void Up()
	{		
		Create.Table(SharedProductsCustomizationTable)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("productGuid").AsFixedLengthString(36).NotNullable().ForeignKey("shared_products", "guid")
			.WithColumn("userGuid").AsFixedLengthString(36).NotNullable().ForeignKey("users", "guid")
			.WithColumn("categoryGuid").AsFixedLengthString(36).Nullable().WithDefaultValue(null).ForeignKey("product_categories", "guid")
			.WithColumn("name").AsString(255).NotNullable();
	}

	private const string SharedProductsCustomizationTable = "shared_products_customization";
}
