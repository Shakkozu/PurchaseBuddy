﻿using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddy.Tests.catalogue.Integration;
internal class ProductCategoryManagementTests : CatalogueTestsFixture
{
	[SetUp]
	public void SetUp()
	{
		userProductsRepo = new InMemoryProductsRepository();
		userCategoriesRepo = new InMemoryUserProductCategoriesRepository();
		userProductsManagementService = new UserProductCategoriesManagementService(userCategoriesRepo, userProductsRepo);
	}

	[Test]
	public void CanAddNewProductCategory()
	{
		var productCategory = AUserProductCategory();

		userProductsManagementService.AddNewProductCategory(productCategory);
		var productCategories = userProductsManagementService.GetUserProductCategories(UserId);

		Assert.Contains(productCategory, productCategories);
	}

	[Test]
	public void WhenProductAddedToCategory_AssertProductIsShownInCategory()
	{
		var productCategory = ProductCategoryCreated("dairy");
		var product = ProductCreated("cheese");

		userProductsManagementService.AssignUserProductToCategory(UserId, product.Guid, productCategory.Guid);

		var categoryFromDb = userProductsManagementService.GetUserProductCategories(UserId).First();
		Assert.NotNull(categoryFromDb);
		Assert.True(categoryFromDb.ContainsProductWithGuid(product.Guid));
	}

	private UserProductCategory ProductCategoryCreated(string name)
	{
		var category = UserProductCategory.CreateNew(name, UserId);
		return userCategoriesRepo.Save(category);
	}

	private IProduct ProductCreated(string name)
	{
		var product = UserProduct.Create(name, UserId);
		return userProductsRepo.Save(product);
	}


	private InMemoryProductsRepository userProductsRepo;
	private InMemoryUserProductCategoriesRepository userCategoriesRepo;
	private UserProductCategoriesManagementService userProductsManagementService;
}
