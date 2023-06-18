using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.infra;
public class SeedSharedProductsDatabase
{
	private readonly IProductsRepository productsRepository;

	public SeedSharedProductsDatabase(IProductsRepository productsRepository)
	{
		this.productsRepository = productsRepository;
	}
	public void Seed()
	{
		if (AlreadySeed())
			return;

	}

	private bool AlreadySeed()
	{
		return false;
		return productsRepository.GetSharedProducts().Any();
	}

	private const string baseCategories =
@"
Food products:
	Beverages
	Bakery products
	Dairy products:
		Milk
		Cheese
	Meat products
	Fruits
	Vegetables
	Sweets
";
	private const string baseProducts = 
@"Bread; Food products
Milk; Food products
Apples; Food products
Chicken breast; Food products
Cheese; Food products";
}
