using PurchaseBuddy.src.catalogue.App;
using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddyLibrary.src.infra;
public class SeedSharedProductsDatabase
{
	private readonly UserProductsManagementService userProductsService;
	private readonly UserProductCategoriesManagementService categoriesManagementService;
	private readonly IProductsRepository productsRepository;
	private readonly IUserProductCategoriesRepository productCategoriesRepository;

	public SeedSharedProductsDatabase(UserProductsManagementService userProductsService,
		UserProductCategoriesManagementService categoriesManagementService,
		IProductsRepository productsRepository,
		IUserProductCategoriesRepository productCategoriesRepository)
	{
		this.userProductsService = userProductsService;
		this.categoriesManagementService = categoriesManagementService;
		this.productsRepository = productsRepository;
		this.productCategoriesRepository = productCategoriesRepository;
	}
	public void Seed()
	{
		if (AlreadySeed())
			return;

		var categories = baseCategories.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		// get categories from baseCategories string. Tabs means, that category is nested on level of tabs count
		var categoriesList = new List<IProductCategory>();
		foreach (var category in categories)
		{
			var tabsCount = category.Count(c => c == '\t');
			var categoryName = category.TrimStart('\t').Replace(":", string.Empty);
			var parentCategory = categoriesList.FirstOrDefault(c => c.Name == categoryName);
			if (parentCategory == null)
			{
				parentCategory = SharedProductCategory.CreateNew(categoryName);
				categoriesList.Add(parentCategory);
			}
			for (var i = 0; i < tabsCount; i++)
			{
				var childCategory = SharedProductCategory.CreateNew(categoryName);
				parentCategory.AddChild(childCategory);
			}
		}
		foreach (var category in categoriesList)
			productCategoriesRepository.Save(category);


	}

	private bool AlreadySeed()
	{
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
