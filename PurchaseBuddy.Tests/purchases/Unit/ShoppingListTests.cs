using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;

namespace PurchaseBuddy.Tests.purchases.Unit;
internal class ShoppingListTests : Fixture
{
    [Test]
    public void ShouldUpdateListItems_NotChangedItemsAreNotModified()
    {
        var shoppingList = AShoppingList();
		var firstItem = ShoppingListItem.CreateNew(Guid.NewGuid());
		var items = new List<ShoppingListItem> { firstItem, ShoppingListItem.CreateNew(Guid.NewGuid()) };
		shoppingList.AddNew(firstItem);
		shoppingList.AddNew(ShoppingListItem.CreateNew(Guid.NewGuid()));
		shoppingList.MarkListItemAsPurchased(firstItem.Guid);

        shoppingList.UpdateListItems(items);

        Assert.True(shoppingList.Items.First(item => item.Guid == firstItem.Guid).Purchased);
    }
	
	[Test]
    public void ShouldUpdateListItems()
    {
        var productId = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var shoppingList = AShoppingList();
		var items = new List<ShoppingListItem> { ShoppingListItem.CreateNew(productId2), ShoppingListItem.CreateNew(productId) };
		shoppingList.AddNew(ShoppingListItem.CreateNew(productId));

        shoppingList.UpdateListItems(items);

        Assert.True(shoppingList.Items.All(item => item.ProductId == productId2));
		Assert.False(shoppingList.Items.First().Purchased);
    }
	
	[Test]
    public void MarkProductAsPurchased_AssertFlagUpdated()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid());
        shoppingList.AddNew(shoppingListItem);

        shoppingList.MarkListItemAsPurchased(shoppingListItem.Guid);

        Assert.True(shoppingList.Items.First().Purchased);
        Assert.False(shoppingList.Items.First().Unavailable);
    }

    [Test]
    public void MarkProductAsUnavailable_AssertFlagUpdated()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid());
        shoppingList.AddNew(shoppingListItem);

        shoppingList.MarkListItemAsUnavailable(shoppingListItem.Guid);

        Assert.True(shoppingList.Items.First().Unavailable);
        Assert.False(shoppingList.Items.First().Purchased);
    }

    [Test]
    public void AddItemToShoppingList()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId);

        shoppingList.AddNew(shoppingListItem);

        Assert.True(shoppingList.Items.Contains(shoppingListItem));
    }

    [Test]
    public void WhenAddedItemAlreadyExistsOnList_IncreaseQuantity()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId);
        var shoppingListItem2 = ShoppingListItem.CreateNew(productId);

        shoppingList.AddNew(shoppingListItem);
        shoppingList.AddNew(shoppingListItem2);

        Assert.AreEqual(2, shoppingList.Items.First().Quantity);
    }

    [Test]
    public void WhenAddingItemWithSpecifiedQuantity_AssertQuantityMatch()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);

        shoppingList.AddNew(shoppingListItem);

        Assert.AreEqual(5, shoppingList.Items.First().Quantity);
    }


    [Test]
    public void WhenAddingDifferentItems_AssertListAddSeparatePosition()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);
        var shoppingListItem2 = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem);
        shoppingList.AddNew(shoppingListItem2);

        Assert.AreEqual(2, shoppingList.Items.Count);
    }

    [Test]
    public void ChangeQuantityOf_AssertChanges()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem);
        shoppingList.ChangeQuantityOf(shoppingListItem.Guid, 10);

        Assert.AreEqual(10, shoppingList.Items.First().Quantity);
    }

    [Test]
    public void ChangeQuantityTo_WhenProductDoesNotExistsOnList_Ignore()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);

        Assert.DoesNotThrow(() => shoppingList.ChangeQuantityOf(productId, 10));
    }

    [Test]
    public void RemoveItem_AssertRemovedFromList()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);
        var shoppingListItem2 = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem);
        shoppingList.AddNew(shoppingListItem2);
        Assert.AreEqual(2, shoppingList.Items.Count);

        shoppingList.Remove(shoppingListItem.Guid);
        Assert.AreEqual(1, shoppingList.Items.Count);

        shoppingList.Remove(shoppingListItem2.Guid);
        Assert.AreEqual(0, shoppingList.Items.Count);
    }
}
