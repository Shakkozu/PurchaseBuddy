using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(3, "Create product tables")]
public class Upgrade0003_CreateProductTables : Migration
{
	public override void Down()
	{
		Delete.Table(SharedProductsTableName);
		Delete.Table(UserProductsTableName);
	}

	public override void Up()
	{
		Create.Table(SharedProductsTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(36).NotNullable().Unique()
			.WithColumn("categoryGuid").AsFixedLengthString(36).Nullable().WithDefaultValue(null)
			.WithColumn("name").AsString(255).NotNullable();
		
		Create.ForeignKey("FK_SharedProducts_CategoryGuid_ProductCategories_Guid")
			.FromTable(SharedProductsTableName).ForeignColumn("categoryGuid")
			.ToTable(ProductCategoriesTableName).PrimaryColumn("guid");

		Create.Table(UserProductsTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(36).NotNullable().Unique()
			.WithColumn("categoryGuid").AsFixedLengthString(36).Nullable().WithDefaultValue(null)
			.WithColumn("userGuid").AsFixedLengthString(36).NotNullable()
			.WithColumn("name").AsString(255).NotNullable();

		Create.ForeignKey("FK_UserProducts_CategoryGuid_ProductCategories_Guid")
			.FromTable(UserProductsTableName).ForeignColumn("categoryGuid")
			.ToTable(ProductCategoriesTableName).PrimaryColumn("guid");

		Create.ForeignKey("FK_UserProducts_UserGuid_Users_Guid")
			.FromTable(UserProductsTableName).ForeignColumn("userGuid")
			.ToTable("users").PrimaryColumn("guid");
	}

	private const string SharedProductsTableName = "shared_products";
	private const string UserProductsTableName = "user_products";
	private const string ProductCategoriesTableName = "product_categories";
}
