using PurchaseBuddy.src.catalogue.Model;

namespace PurchaseBuddy.Tests.catalogue.Unit;
internal class UserProductCategoryTests : CatalogueTestsFixture
{
	[Test]
	public void CanAddProductToCategory()
	{
		var category = AUserProductCategory();
		var product = UserProduct.Create("eggs XL 8x", UserId);

		category.AddProduct(product);

		Assert.True(category.ContainsProductWithGuid(product.Guid));
	}
	
	[Test]
	public void WhenProductIsAddedToChildCategory_AssertProductIsFoundOnCategoryProductsList()
	{
		var parentCategory = AUserProductCategory();
		var childCategory = AUserProductCategoryWithParent(parentCategory);
		var product = UserProduct.Create("eggs XL 8x", UserId);

		childCategory.AddProduct(product);

		Assert.True(parentCategory.ContainsProductWithGuid(product.Guid));
		Assert.True(childCategory.ContainsProductWithGuid(product.Guid));
	}

	[Test]
	public void WhenGroupContainsProductsAndChildrenWithProducts_ReturnAllProducts()
	{
		var parentCategory = AUserProductCategory("nabiał");
		parentCategory.AddProduct(UserProduct.Create("eggs", UserId));
		var childCategory = AUserProductCategoryWithParent(parentCategory, "sery");
		childCategory.AddProduct(UserProduct.Create("twaróg", UserId));
		childCategory.AddProduct(UserProduct.Create("mozarella", UserId));

		Assert.AreEqual(3, parentCategory.GetProductsInCategory().Count);
		Assert.AreEqual(2, childCategory.GetProductsInCategory().Count);
	}

	[Test]
	public void WhenProductIsAddedToCategory_WhenProductExistInChildrenCategoryProductsList_MoveProductToSpecifiedCategory()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		var childCategory = AUserProductCategoryWithParent(parentCategory, "cheese");

		childCategory.AddProduct(product);

		Assert.True(parentCategory.ContainsProductWithGuid(product.Guid));
		Assert.True(childCategory.ContainsProductWithGuid(product.Guid));
	}

	[Test]
	public void WhenProductRemoved_AssertRemovedCorrectly()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		parentCategory.AddProduct(product);

		parentCategory.RemoveProduct(product);

		Assert.False(parentCategory.ContainsProductWithGuid(product.Guid));
	}

	[Test]
	public void WhenParentCategoryContainsProduct_AssertProductIsNotAdded()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		var childCategory = AUserProductCategoryWithParent(parentCategory, "cheese");
		parentCategory.AddProduct(product);

		childCategory.AddProduct(product);

		Assert.True(parentCategory.ContainsProductWithGuid(product.Guid));
		Assert.False(childCategory.ContainsProductWithGuid(product.Guid));
	}
	
	[Test]
	public void WhenParentCategoryContainsProductOnMoreNestedExample_AssertProductIsNotAdded()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		var childCategory = AUserProductCategoryWithParent(parentCategory, "cheese");
		var grandChildCategory = AUserProductCategoryWithParent(childCategory, "italian cheese");
		parentCategory.AddProduct(product);

		grandChildCategory.AddProduct(product);

		Assert.AreEqual(1, parentCategory.GetProductsInCategory().Count);
		Assert.AreEqual(0, grandChildCategory.GetProductsInCategory().Count);
		Assert.AreEqual(0, childCategory.GetProductsInCategory().Count);
	}

	[Test]
	public void WhenChildContainsProduct_AssertProductIsNotAdded()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		var childCategory = AUserProductCategoryWithParent(parentCategory, "cheese");
		childCategory.AddProduct(product);

		parentCategory.AddProduct(product);

		Assert.AreEqual(1, parentCategory.GetProductsInCategory().Count);
		Assert.AreEqual(1, childCategory.GetProductsInCategory().Count);
	}
	
	[Test]
	public void WhenGrandChildContainsProduct_AssertProductIsNotAdded()
	{
		var product = UserProduct.Create("mozarella", UserId);
		var parentCategory = AUserProductCategory("dairy");
		var childCategory = AUserProductCategoryWithParent(parentCategory, "cheese");
		var grandChildCategory = AUserProductCategoryWithParent(childCategory, "italian cheese");
		grandChildCategory.AddProduct(product);

		parentCategory.AddProduct(product);

		Assert.AreEqual(1, parentCategory.GetProductsInCategory().Count);
		Assert.AreEqual(1, childCategory.GetProductsInCategory().Count);
	}
}
