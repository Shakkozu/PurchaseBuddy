using PurchaseBuddy.src.purchases.persistance;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using PurchaseBuddyLibrary.purchases.domain;
using PurchaseBuddyLibrary.purchases.persistance;

namespace PurchaseBuddy.src.purchases.domain;

public class ShoppingList
{
	private readonly List<ShoppingListItem> shoppingListItems;
	private readonly List<Guid> _usersAllowedToModify;

	public IReadOnlyCollection<Guid> UsersAllowedToModify => new ReadOnlyCollection<Guid>(_usersAllowedToModify);

	public IReadOnlyCollection<ShoppingListItem> Items => new ReadOnlyCollection<ShoppingListItem>(shoppingListItems);
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
					  DateTime createdAt,
					  DateTime? closedAt,
					  List<Guid> usersAllowedToModify)
	{
		UserId = userId;
		shoppingListItems = shoppingListEntries;
		CreatedAt = createdAt;
		CompletedAt = closedAt;
		Guid = guid;
		ShopId = shopId;
		_usersAllowedToModify = usersAllowedToModify ?? new List<Guid>();

	}
	public static ShoppingList CreateNew(Guid userId, Guid? shopId = null)
	{
		return new ShoppingList(userId,
						  shopId,
						  Guid.NewGuid(),
						  new List<ShoppingListItem>(),
						  DateTime.UtcNow,
						  null,
						  new List<Guid>());
	
	}
	public static ShoppingList CreateNew(Guid userId, List<ShoppingListItem> items, Guid? shopId = null)
	{
		if (!items.Any())
			throw new InvalidOperationException("cannot create empty list");

		var shoppingList = new ShoppingList(userId,
									  shopId,
									  Guid.NewGuid(),
									  new List<ShoppingListItem>(),
									  DateTime.UtcNow,
									  null,
									  new List<Guid>());
		foreach(var item in items)
			shoppingList.AddNew(item);

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
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsPurchased();
		if (!shoppingListItems.Any(listItem => !listItem.Purchased))
			Complete();
	}

	internal void MarkListItemAsNotPurchased(Guid listItemId)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsNotPurchased();
	}

	public void MarkListItemAsUnavailable(Guid listItemId)
	{
		var toUpdate = shoppingListItems.FirstOrDefault(listItem => listItem.Guid == listItemId);
		if (toUpdate is null)
			return;

		toUpdate.MarkAsUnavailable();
	}

	public void AddNew(ShoppingListItem shoppingListItem)
	{
		if (IsCompleted)
			throw new InvalidOperationException("cannot add item to completed list");

		if(shoppingListItem is ImportedShoppingListItem imported)
		{
			var itemToIncrement = Items.FirstOrDefault(item => item is ImportedShoppingListItem importedItem && importedItem.ProductName.Trim() == imported.ProductName.Trim());
			if(itemToIncrement != null)
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

	public void ChangeQuantityOf(Guid listItemId, int newQuantity)
	{
		var itemToUpdate = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (itemToUpdate == null)
			return;

		itemToUpdate.ChangeQuantityTo(newQuantity);
	}

	public void Remove(Guid listItemId)
	{
		var itemToRemove = shoppingListItems.Find(listItem => listItem.Guid == listItemId);
		if (itemToRemove == null)
			return;

		shoppingListItems.Remove(itemToRemove);
		if (shoppingListItems.TrueForAll(listItem => listItem.Purchased))
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
			Remove(itemToRemove.Guid);

		return CreateNew(UserId, notBoughtProducts, shopId);
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
			dao.CreatedAt,
			dao.CompletedAt,
			dao.GetAllowedUsers()
			);
	}

	public void UpdateListItems(IEnumerable<ShoppingListItem> newItemsList)
	{
		var productsToRemove = Items
			.Where(existingItem => !newItemsList.Any(newItem => newItem.Guid == existingItem.Guid))
			.Select(x => x.Guid)
			.ToImmutableList();
		var productsToAdd = newItemsList
			.Where(newItem => !Items.Any(existingItem => existingItem.Guid == newItem.Guid))
			.ToImmutableList();

		foreach (var productToAdd in productsToAdd)
			AddNew(productToAdd);
		foreach (var productToRemove in productsToRemove)
			Remove(productToRemove);
	}

	internal void ShareTo(Guid otherUser)
	{
		if (_usersAllowedToModify.Contains(otherUser))
			return;

		_usersAllowedToModify.Add(otherUser);
	}
}
