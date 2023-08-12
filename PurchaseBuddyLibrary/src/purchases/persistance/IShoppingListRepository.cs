using PurchaseBuddy.src.purchases.domain;

namespace PurchaseBuddy.src.purchases.persistance;

public interface IShoppingListRepository
{
	IList<ShoppingList> GetAll(Guid userId);
	ShoppingList? GetShoppingList(Guid userId, Guid shoppingListGuid);
	ShoppingList? GetShoppingList(Guid shoppingListGuid);
	void Save(ShoppingList shoppingList);
	void Update(ShoppingList shoppingList);
}
