﻿using PurchaseBuddy.src.catalogue.Model;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddy.src.purchases.persistance;

namespace PurchaseBuddy.src.purchases.app;

public class ShoppingListProductsManagementService
{
	private IShoppingListRepository shoppingListRepository;
	private IUserProductsRepository userProductsRepository;

	public ShoppingListProductsManagementService(IShoppingListRepository shoppingListRepository, IUserProductsRepository userProductsRepository)
	{
		this.shoppingListRepository = shoppingListRepository;
		this.userProductsRepository = userProductsRepository;
	}
	public void MarkProductAsUnavailable(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsUnavailable(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void MarkProductAsPurchased(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.MarkProductAsPurchased(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void AddProductToList(Guid userId, Guid shoppingListId, UserProduct userProduct)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		var savedProduct = userProductsRepository.GetUserProduct(userId, userProduct.Guid);
		if(savedProduct is null)
			savedProduct = userProductsRepository.Save(userProduct);

		var shoppingListItem = new ShoppingListItem(savedProduct.Guid);
		shoppingList.AddNew(shoppingListItem);
		shoppingListRepository.Save(shoppingList);
	}
	public void RemoveProductFromList(Guid userId, Guid shoppingListId, Guid productId)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.Remove(productId);
		shoppingListRepository.Save(shoppingList);
	}
	public void ChangeQuantityOfProductOnList(Guid userId, Guid shoppingListId, Guid productId, int newQuantity)
	{
		var shoppingList = shoppingListRepository.GetShoppingList(userId, shoppingListId);
		shoppingList.ChangeQuantityOf(productId, newQuantity);
		shoppingListRepository.Save(shoppingList);
	}
}