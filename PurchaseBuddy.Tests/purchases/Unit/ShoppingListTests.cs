using PurchaseBuddy.src.purchases.domain;
using PurchaseBuddyLibrary.purchases.domain;

namespace PurchaseBuddy.Tests.purchases.Unit;
internal class ShoppingListTests : Fixture
{
	[Test]
    public void MarkProductAsPurchased_AssertFlagUpdated()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid());
        shoppingList.AddNew(shoppingListItem, UserId);

        shoppingList.MarkListItemAsPurchased(shoppingListItem.Guid);

        Assert.True(shoppingList.Items.First().Purchased);
        Assert.False(shoppingList.Items.First().Unavailable);
    }

    [Test]
    public void MarkProductAsUnavailable_AssertFlagUpdated()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid());
        shoppingList.AddNew(shoppingListItem, UserId);

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

        shoppingList.AddNew(shoppingListItem, UserId);

        Assert.True(shoppingList.Items.Contains(shoppingListItem));
    }

    [Test]
    public void WhenAddedItemAlreadyExistsOnList_IncreaseQuantity()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId);
        var shoppingListItem2 = ShoppingListItem.CreateNew(productId);

        shoppingList.AddNew(shoppingListItem, UserId);
        shoppingList.AddNew(shoppingListItem2, UserId);

        Assert.AreEqual(2, shoppingList.Items.First().Quantity);
    }

    [Test]
    public void WhenAddingItemWithSpecifiedQuantity_AssertQuantityMatch()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);

        shoppingList.AddNew(shoppingListItem, UserId);

        Assert.AreEqual(5, shoppingList.Items.First().Quantity);
    }


    [Test]
    public void WhenAddingDifferentItems_AssertListAddSeparatePosition()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);
        var shoppingListItem2 = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem, UserId);
        shoppingList.AddNew(shoppingListItem2, UserId);

        Assert.AreEqual(2, shoppingList.Items.Count);
    }

    [Test]
    public void ChangeQuantityOf_AssertChanges()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem, UserId);
        shoppingList.ChangeQuantityOf(shoppingListItem.Guid, 10, UserId);

        Assert.AreEqual(10, shoppingList.Items.First().Quantity);
    }

    [Test]
    public void ChangeQuantityTo_WhenProductDoesNotExistsOnList_Ignore()
    {
        var productId = Guid.NewGuid();
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(productId, 5);

        Assert.DoesNotThrow(() => shoppingList.ChangeQuantityOf(productId, 10, UserId));
    }

    [Test]
    public void RemoveItem_AssertRemovedFromList()
    {
        var shoppingList = AShoppingList();
        var shoppingListItem = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);
        var shoppingListItem2 = ShoppingListItem.CreateNew(Guid.NewGuid(), 5);

        shoppingList.AddNew(shoppingListItem, UserId);
        shoppingList.AddNew(shoppingListItem2, UserId);
        Assert.AreEqual(2, shoppingList.Items.Count);

        shoppingList.Remove(shoppingListItem.Guid, UserId);
        Assert.AreEqual(1, shoppingList.Items.Count);

        shoppingList.Remove(shoppingListItem2.Guid, UserId);
        Assert.AreEqual(0, shoppingList.Items.Count);
    }
}
