using FluentMigrator;

namespace PurchaseBuddy.Database.Migrations;

[Migration(2, "Create product categories")]
public class Upgrade0002_CreateProductCategoriesTable : Migration
{
	private const string ProductCategoriesTableName = "product_categories";
	private const string ProductCategoriesHierarchyTableName = "product_categories_hierarchy";

	public override void Down()
	{
		Delete.Table(ProductCategoriesHierarchyTableName);
		Delete.Table(ProductCategoriesTableName);
	}

	public override void Up()
	{
		Create.Table(ProductCategoriesTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).NotNullable().Unique()
			.WithColumn("user_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).Nullable().ForeignKey("users", "guid")
			.WithColumn("name").AsString().NotNullable()
			.WithColumn("description").AsString().Nullable();

		Create.Table(ProductCategoriesHierarchyTableName)
			.WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
			.WithColumn("category_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).ForeignKey(ProductCategoriesTableName, "guid")
			.WithColumn("user_guid").AsFixedLengthString(MigrationHelper.GuidColumnLength).Nullable().ForeignKey("users", "guid")
			.WithColumn("root_path").AsString();
	}
}