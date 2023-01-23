using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.Tests.purchases.Unit;
[TestFixture]
internal class Fixture
{
	protected Guid UserId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");

	public ShoppingList AShoppingList()
	{
		return ShoppingList.CreateNew(UserId);
	}
}
internal class ShoppingListTests : Fixture
{
	[Test]
	public void MarkProductAsPurchased_AssertFlagUpdated()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId);
		shoppingList.AddNew(shoppingListItem);

		shoppingList.MarkProductAsPurchased(productId);

		Assert.True(shoppingList.Items.First().Purchased);
		Assert.False(shoppingList.Items.First().Unavailable);
	}

	[Test]
	public void MarkProductAsUnavailable_AssertFlagUpdated()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId);
		shoppingList.AddNew(shoppingListItem);

		shoppingList.MarkProductAsUnavailable(productId);

		Assert.True(shoppingList.Items.First().Unavailable);
		Assert.False(shoppingList.Items.First().Purchased);
	}
	
	[Test]
	public void AddItemToShoppingList()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId);

		shoppingList.AddNew(shoppingListItem);

		Assert.True(shoppingList.Items.Contains(shoppingListItem));
	}
	
	[Test]
	public void WhenAddedItemAlreadyExistsOnList_IncreaseQuantity()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId);
		var shoppingListItem2 = new ShoppingListItem(productId);

		shoppingList.AddNew(shoppingListItem);
		shoppingList.AddNew(shoppingListItem2);

		Assert.AreEqual(2, shoppingList.Items.First().Quantity);
	}
	
	[Test]
	public void WhenAddingItemWithSpecifiedQuantity_AssertQuantityMatch()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId, 5);

		shoppingList.AddNew(shoppingListItem);

		Assert.AreEqual(5, shoppingList.Items.First().Quantity);
	}


	[Test]
	public void WhenAddingDifferentItems_AssertListAddSeparatePosition()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId, 5);
		var shoppingListItem2 = new ShoppingListItem(Guid.NewGuid(), 5);

		shoppingList.AddNew(shoppingListItem);
		shoppingList.AddNew(shoppingListItem2);

		Assert.AreEqual(2, shoppingList.Items.Count);
	}
	
	[Test]
	public void ChangeQuantityOf_AssertChanges()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId, 5);

		shoppingList.AddNew(shoppingListItem);
		shoppingList.ChangeQuantityOf(shoppingListItem.ProductId, 10);

		Assert.AreEqual(10, shoppingList.Items.First().Quantity);
	}
	
	[Test]
	public void ChangeQuantityTo_WhenProductDoesNotExistsOnList_Ignore()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId, 5);

		Assert.DoesNotThrow(() => shoppingList.ChangeQuantityOf(productId, 10));
	}


	[Test]
	public void RemoveItem_AssertRemovedFromList()
	{
		var productId = Guid.NewGuid();
		var shoppingList = AShoppingList();
		var shoppingListItem = new ShoppingListItem(productId, 5);
		var shoppingListItem2 = new ShoppingListItem(Guid.NewGuid(), 5);

		shoppingList.AddNew(shoppingListItem);
		shoppingList.AddNew(shoppingListItem2);
		Assert.AreEqual(2, shoppingList.Items.Count);

		shoppingList.Remove(shoppingListItem.ProductId);
		Assert.AreEqual(1, shoppingList.Items.Count);

		shoppingList.Remove(shoppingListItem2.ProductId);
		Assert.AreEqual(0, shoppingList.Items.Count);
	}
}
