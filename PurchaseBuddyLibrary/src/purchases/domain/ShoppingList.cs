﻿using System.Collections.ObjectModel;

namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingList
{
	private readonly List<ShoppingListItem> shoppingListItems;

	public IReadOnlyCollection<ShoppingListItem> Items => new ReadOnlyCollection<ShoppingListItem>(shoppingListItems);
	public Guid UserId { get; }
	public Guid Guid { get; }
	public Guid? ShopId { get; }
	public DateTime CreatedAt { get; }

	public bool IsCompleted => CompletedAt.HasValue;

	public DateTime? CompletedAt { get; private set; }


	private ShoppingList(Guid userId, Guid? shopId, Guid guid, List<ShoppingListItem> shoppingListEntries, DateTime createdAt, DateTime? closedAt)
	{
		UserId = userId;
		shoppingListItems = shoppingListEntries;
		CreatedAt = createdAt;
		CompletedAt = closedAt;
		Guid = guid;
		ShopId = shopId;

	}
	public static ShoppingList CreateNew(Guid userId, Guid? shopId = null)
	{
		return new ShoppingList(userId, shopId, Guid.NewGuid(), new List<ShoppingListItem>(), DateTime.UtcNow, null);
	
	}
	public static ShoppingList CreateNew(Guid userId, List<ShoppingListItem> items, Guid? shopId = null)
	{
		if (!items.Any())
			throw new InvalidOperationException("cannot create empty list");
		var shoppingList = new ShoppingList(userId, shopId, Guid.NewGuid(), new List<ShoppingListItem>(), DateTime.UtcNow, null);
		foreach(var item in items)
			shoppingList.AddNew(item);

		return shoppingList;
	}

	public void Complete()
	{
		if (IsCompleted)
			return;

		CompletedAt = DateTime.UtcNow;
	}

	public void MarkProductAsPurchased(Guid productId)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsPurchased();
		if (!shoppingListItems.Any(listItem => !listItem.Purchased))
			Complete();
	}

	internal void MarkProductAsNotPurchased(Guid productId)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsNotPurchased();
	}

	public void MarkProductAsUnavailable(Guid productId)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsUnavailable();
	}

	public void AddNew(ShoppingListItem shoppingListItem)
	{
		if (IsCompleted)
			throw new InvalidOperationException("cannot add item to completed list");

		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == shoppingListItem.ProductId);
		if (toUpdate is null)
		{
			shoppingListItems.Add(shoppingListItem);
			return;
		}

		toUpdate.ChangeQuantityTo(shoppingListItem.Quantity + toUpdate.Quantity);
	}

	public void ChangeQuantityOf(Guid productId, int newQuantity)
	{
		var itemToUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (itemToUpdate == null)
			return;

		itemToUpdate.ChangeQuantityTo(newQuantity);
	}

	public void Remove(Guid productId)
	{
		var itemToRemove = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (itemToRemove == null)
			return;

		shoppingListItems.Remove(itemToRemove);
		if (shoppingListItems.All(listItem => listItem.Purchased))
			Complete();
	}

	internal ShoppingList GenerateNewWithNotBoughtItems(Guid shopId)
	{
		var notBoughtProducts = shoppingListItems
			.Where(listItem => !listItem.Purchased)
			.ToList();
		if (!notBoughtProducts.Any())
			throw new InvalidOperationException("cannnot create list when source is completed");

		foreach (var itemToRemove in notBoughtProducts)
			Remove(itemToRemove.ProductId);

		return CreateNew(UserId, notBoughtProducts, shopId);
	}
}
