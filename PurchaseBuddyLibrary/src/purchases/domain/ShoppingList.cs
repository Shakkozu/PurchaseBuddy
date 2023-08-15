using PurchaseBuddy.src.purchases.persistance;
using System.Collections.ObjectModel;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.purchases.persistance;

namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingList
{
	private readonly List<ShoppingListItem> shoppingListItems;
	private readonly List<Guid> _allowedUsers = new();

	public IReadOnlyCollection<ShoppingListItem> Items => new ReadOnlyCollection<ShoppingListItem>(shoppingListItems);
	public IReadOnlyCollection<Guid> UsersAllowedToModify => new ReadOnlyCollection<Guid>(_allowedUsers);
	public Guid UserId { get; }
	public Guid Guid { get; }
	public Guid? ShopId { get; }
	public DateTime CreatedAt { get; }
	public bool IsCompleted => CompletedAt.HasValue;
	public DateTime? CompletedAt { get; private set; }


	private ShoppingList(Guid userId,
					  Guid? shopId,
					  Guid guid,
					  List<ShoppingListItem> shoppingListEntries,
					  List<Guid> usersAllowedToModify,
					  DateTime createdAt,
					  DateTime? closedAt
					  )
	{
		UserId = userId;
		shoppingListItems = shoppingListEntries;
		_allowedUsers = usersAllowedToModify ?? new List<Guid>();
		CreatedAt = createdAt;
		CompletedAt = closedAt;
		Guid = guid;
		ShopId = shopId;

	}
	public static ShoppingList CreateNew(Guid userId, Guid? shopId = null)
	{
		return new ShoppingList(userId,
						  shopId,
						  Guid.NewGuid(),
						  new List<ShoppingListItem>(),
						  new List<Guid>(),
						  DateTime.UtcNow,
						  null);

	}
	public static ShoppingList CreateNew(Guid userId, List<ShoppingListItem> items, Guid? shopId = null)
	{
		if (!items.Any())
			throw new InvalidOperationException("cannot create empty list");

		var shoppingList = new ShoppingList(userId,
									  shopId,
									  Guid.NewGuid(),
									  new List<ShoppingListItem>(),
									  new List<Guid>(),
									  DateTime.UtcNow,
									  null);
		foreach (var item in items)
			shoppingList.AddNew(item, userId);

		return shoppingList;
	}

	public void Complete()
	{
		if (IsCompleted)
			return;

		CompletedAt = DateTime.Now;
	}

	public void MarkListItemAsPurchased(Guid listItemId)
	{
		var toUpdate = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsPurchased();
		if (!shoppingListItems.Exists(listItem => !listItem.Purchased))
			Complete();
	}

	internal void MarkListItemAsNotPurchased(Guid listItemId)
	{
		var toUpdate = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsNotPurchased();
	}

	public void MarkListItemAsUnavailable(Guid listItemId)
	{
		var toUpdate = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsUnavailable();
	}

	public void AddNew(ShoppingListItem shoppingListItem, Guid userId)
	{
		if (IsCompleted)
			throw new InvalidOperationException("cannot add item to completed list");
		if (!UserHasAccessToModifyingList(userId))
			throw new InvalidOperationException($"User {userId} does not have access to list {Guid}");

		if (shoppingListItem is ImportedShoppingListItem imported)
		{
			var itemToIncrement = Items.FirstOrDefault(item => item is ImportedShoppingListItem importedItem && importedItem.ProductName.Trim() == imported.ProductName.Trim());
			if (itemToIncrement != null)
				itemToIncrement.ChangeQuantityTo(itemToIncrement.Quantity + shoppingListItem.Quantity);
			else
				shoppingListItems.Add(imported);
			return;
		}
		var itemToUpdate = Items.FirstOrDefault(item => item.ProductId == shoppingListItem.ProductId || item.Guid == shoppingListItem.Guid);
		if (itemToUpdate != null)
			itemToUpdate.ChangeQuantityTo(shoppingListItem.Quantity + itemToUpdate.Quantity);
		else
			shoppingListItems.Add(shoppingListItem);
	}

	private bool UserHasAccessToModifyingList(Guid userId)
	{
		return userId == UserId || _allowedUsers.Contains(userId);
	}

	public void ChangeQuantityOf(Guid listItemId, int newQuantity, Guid userId)
	{
		if (!UserHasAccessToModifyingList(userId))
			throw new InvalidOperationException($"User {userId} does not have access to list {Guid}");
		var itemToUpdate = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (itemToUpdate == null)
			return;

		itemToUpdate.ChangeQuantityTo(newQuantity);
	}

	public void Remove(Guid listItemId, Guid userId)
	{
		if (!UserHasAccessToModifyingList(userId))
			throw new InvalidOperationException($"User {userId} does not have access to list {Guid}");
		var itemToRemove = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (itemToRemove == null)
			return;

		shoppingListItems.Remove(itemToRemove);
		if (shoppingListItems.TrueForAll(listItem => listItem.Purchased))
			Complete();
	}

	internal static ShoppingList LoadFrom(ShoppingListDao dao)
	{
		var shoppingListEntries = dao.GetShoppingListEntries()
			.Select(listItemDao => listItemDao is ImportedShoppingListItemDao importedDao ?
				ImportedShoppingListItem.LoadFrom(importedDao) :
				ShoppingListItem.LoadFrom((UserShoppingListItemDao)listItemDao))
			.ToList();

		return new ShoppingList(
			Guid.Parse(dao.UserGuid),
			string.IsNullOrEmpty(dao.ShopGuid) ? (Guid?)null : Guid.Parse(dao.ShopGuid),
			Guid.Parse(dao.Guid),
			shoppingListEntries,
			dao.GetUsersAllowedToModify(),
			dao.CreatedAt,
			dao.CompletedAt
			);
	}

	internal void GrantAccessToModifyingTo(Guid userId)
	{
		if (userId == UserId)
			return;
		if (_allowedUsers.Contains(userId))
			return;

		_allowedUsers.Add(userId);
	}
}
