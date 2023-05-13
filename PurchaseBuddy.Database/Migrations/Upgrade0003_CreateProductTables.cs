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
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("category_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).Nullable().WithDefaultValue(null)
			.WithColumn("name").AsString(255).NotNullable();
		
		Create.ForeignKey("FK_SharedProducts_CategoryGuid_ProductCategories_Guid")
			.FromTable(SharedProductsTableName).ForeignColumn("category_guid")
			.ToTable(ProductCategoriesTableName).PrimaryColumn("guid");

		Create.Table(UserProductsTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("category_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).Nullable().WithDefaultValue(null)
			.WithColumn("user_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable()
			.WithColumn("name").AsString(255).NotNullable();

		Create.ForeignKey("FK_UserProducts_CategoryGuid_ProductCategories_Guid")
			.FromTable(UserProductsTableName).ForeignColumn("category_guid")
			.ToTable(ProductCategoriesTableName).PrimaryColumn("guid");

		Create.ForeignKey("FK_UserProducts_UserGuid_Users_Guid")
			.FromTable(UserProductsTableName).ForeignColumn("user_guid")
			.ToTable("users").PrimaryColumn("guid");
	}

	private const string SharedProductsTableName = "shared_products";
	private const string UserProductsTableName = "user_products";
	private const string ProductCategoriesTableName = "product_categories";
}
