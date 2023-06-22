﻿namespace PurchaseBuddyLibrary.src.purchases.app.contract;

public interface IShoppingListReadService
{
	ShoppingListDto GetShoppingList(Guid userId, Guid shoppingListId);
	IList<ShoppingListDto> GetNotClosedShoppingLists(Guid userId);
}