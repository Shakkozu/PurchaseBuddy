using System.Collections.ObjectModel;

namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingList
{
	private readonly List<ShoppingListItem> shoppingListItems;

	public IReadOnlyCollection<ShoppingListItem> Items => new ReadOnlyCollection<ShoppingListItem>(shoppingListItems);
	public Guid UserId { get; }
	public bool IsClosed => closedAt.HasValue;
	private DateTime? closedAt;
	private DateTime createdAt;

	private ShoppingList(Guid userId, List<ShoppingListItem> shoppingListEntries, DateTime createdAt, DateTime? closedAt)
	{
		UserId = userId;
		shoppingListItems = shoppingListEntries;
		this.createdAt = createdAt;
		this.closedAt = closedAt;
	}
	public static ShoppingList CreateNew(Guid userId)
	{
		return new ShoppingList(userId, new List<ShoppingListItem>(), DateTime.UtcNow, null);
	}

	public void Close()
	{
		if (IsClosed)
			return;

		closedAt = DateTime.UtcNow;
	}

	public void AddNew(ShoppingListItem shoppingListItem)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == shoppingListItem.ProductId);
		if(toUpdate is null)
		{
			shoppingListItems.Add(shoppingListItem);
			return;
		}

		shoppingListItems.Remove(toUpdate);
		shoppingListItems.Add(new ShoppingListItem(shoppingListItem.ProductId, toUpdate.Quantity + shoppingListItem.Quantity));
	}

	public void ChangeQuantityOf(Guid productId, int newQuantity)
	{
		var itemToUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (itemToUpdate == null)
			return;

		shoppingListItems.Remove(itemToUpdate);
		shoppingListItems.Add(new ShoppingListItem(productId, newQuantity));
	}

	public void Remove(Guid productId)
	{
		var itemToRemove = shoppingListItems.FirstOrDefault(listItem => listItem.ProductId == productId);
		if (itemToRemove == null)
			return;

		shoppingListItems.Remove(itemToRemove);
	}
}
