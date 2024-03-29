﻿using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.infra;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using PurchaseBuddyLibrary.src.catalogue.Queries.GetUserProducts;

namespace PurchaseBuddy.src.catalogue.App;
public class UserProductsManagementService : IUserProductsManagementService
{
	private readonly IProductsRepository productsRepository;
	private readonly GetUserProductsQueryHandler getUserProductsQueryHandler;
	private readonly GetUserProductsInCategoryQueryHandler getUserProductsInCategoryQuery;
	private readonly IUserProductCategoriesManagementService userProductCategoriesManagementService;

	public UserProductsManagementService(IProductsRepository userProductsRepository,
		IUserProductCategoriesManagementService userProductCategoriesManagementService)
	{
		this.productsRepository = userProductsRepository;
		this.getUserProductsQueryHandler = new GetUserProductsQueryHandler(userProductsRepository, userProductCategoriesManagementService);
		this.getUserProductsInCategoryQuery = new GetUserProductsInCategoryQueryHandler(userProductsRepository, userProductCategoriesManagementService);
		this.userProductCategoriesManagementService = userProductCategoriesManagementService;
	}
	public void ChangeProductCategory(Guid userGuid, Guid productId, Guid? categoryId)
	{
		var product = productsRepository.GetProduct(productId, userGuid);
		if (product == null)
			throw new ResourceNotFoundException($"Product {productId} not found for user {userGuid}");

		if (product.CategoryId.GetValueOrDefault() == categoryId)
			return;

		var category = userProductCategoriesManagementService.GetUserProductCategory(userGuid, categoryId.GetValueOrDefault());
		if (category is null)
			throw new ResourceNotFoundException("Category not found");

		product.AssignProductToCategory(category);
		productsRepository.Update(product, userGuid);
	}

	public IProduct DefineNewUserProduct(UserProduct product)
	{
		return productsRepository.Save(product);
	}
	public IProduct DefineNewUserProduct(UserProductDto productDto, Guid userId)
	{
		if (productDto.CategoryId.HasValue)
		{
			var category = userProductCategoriesManagementService.GetUserProductCategory(userId, productDto.CategoryId.Value);
			if (category is null)
				throw new ResourceNotFoundException($"product category {productDto.CategoryId.Value} not found for user {userId}");
		}

		var product = productDto.CategoryId.HasValue
			? UserProduct.Create(productDto.Name, userId, productDto.CategoryId)
			: UserProduct.Create(productDto.Name, userId);

		return productsRepository.Save(product);
	}

	public List<UserProductDto> GetUserProducts(GetUserProductsQuery query)
	{
		return getUserProductsQueryHandler.Handle(query);
	}
	public List<UserProductDto> GetUserProductsInCategory(GetUserProductsInCategoryQuery query)
	{
		return getUserProductsInCategoryQuery.Handle(query);
	}

	public void Modify(Guid productId, UserProductDto request, Guid userGuid)
	{
		var product = productsRepository.GetProduct(productId, userGuid);
		if (product is null)
			throw new ResourceNotFoundException($"Product {productId} not found for user {userGuid}");

		product.Name = request.Name;
		if (request.CategoryId.GetValueOrDefault() == product.CategoryId.GetValueOrDefault())
		{
			productsRepository.Update(product, userGuid);
			return;
		}

		
		if (!request.CategoryId.HasValue)
		{
			if (product is SharedProduct)
			{
				var customization = new SharedProductCustomization(productId, userGuid, product.Name, request.CategoryId);
				productsRepository.SaveSharedProductCustomization(customization);
				return;
			}
			product.RemoveProductCategory();
			productsRepository.Update(product, userGuid);
			return;
		}

		if (product is SharedProduct)
		{
			var customization = new SharedProductCustomization(productId, userGuid, product.Name, request.CategoryId);
			productsRepository.SaveSharedProductCustomization(customization);
			return;
		}

		var category = userProductCategoriesManagementService.GetUserProductCategory(userGuid, request.CategoryId.Value);
		if (category is null)
			throw new ResourceNotFoundException($"product category {request.CategoryId.Value} not found for user {userGuid}");
		product.AssignProductToCategory(category);
		product.Name = request.Name;
		productsRepository.Update(product, userGuid);
	}

	public void RemoveProductsFromCategory(Guid userGuid, Guid categoryId)
	{
		var category = userProductCategoriesManagementService.GetUserProductCategory(userGuid, categoryId);
		if (category is null)
			throw new ResourceNotFoundException($"product category {categoryId} not found for user {userGuid}");

		var products = productsRepository.GetUserProducts(userGuid).Where(x => x.CategoryId == categoryId);
		foreach (var product in products)
		{
			product.RemoveProductCategory();
			productsRepository.Update(product, userGuid);
		}
	}

	public void ReassignProductsToNewCategory(Guid userGuid, Guid categoryId, Guid newCategoryId)
	{
		var category = userProductCategoriesManagementService.GetUserProductCategory(userGuid, categoryId);
		if (category is null)
			throw new ResourceNotFoundException($"product category {categoryId} not found for user {userGuid}");

		var newCategory = userProductCategoriesManagementService.GetUserProductCategory(userGuid, newCategoryId);
		if (newCategory is null)
			throw new ResourceNotFoundException($"product category {newCategoryId} not found for user {userGuid}");

		var products = productsRepository.GetUserProducts(userGuid).Where(x => x.CategoryId == categoryId);
		foreach (var product in products)
		{
			product.AssignProductToCategory(newCategory);
			productsRepository.Update(product, userGuid);
		}
	}
}
