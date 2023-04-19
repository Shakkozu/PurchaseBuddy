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
