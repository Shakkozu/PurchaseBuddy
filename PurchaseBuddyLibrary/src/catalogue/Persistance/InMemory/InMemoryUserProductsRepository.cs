using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using System.Data.Common;

namespace PurchaseBuddyLibrary.src.catalogue.Persistance.InMemory;

public class InMemoryProductsRepository : IProductsRepository
{
    private readonly Dictionary<Guid, IProduct> products = new();
    private readonly Dictionary<Guid, List<SharedProductCustomization>> sharedProductsCustomizations = new();
    public IProduct? GetProduct(Guid productGuid, Guid userId)
    {
        if (products.ContainsKey(productGuid))
            return products[productGuid];

        return null;
    }

    public void SaveSharedProductCustomization(SharedProductCustomization sharedProductCustomization)
    {
        if (!sharedProductsCustomizations.ContainsKey(sharedProductCustomization.UserID))
        {
            sharedProductsCustomizations[sharedProductCustomization.UserID] = new List<SharedProductCustomization> { sharedProductCustomization };
            return;
        }

        var userCustomizations = sharedProductsCustomizations[sharedProductCustomization.UserID];
        var expectedCustomization = userCustomizations.FirstOrDefault(uc => uc.ProductGuid == sharedProductCustomization.ProductGuid);
        if (expectedCustomization != null)
            userCustomizations.Remove(expectedCustomization);

        userCustomizations.Add(sharedProductCustomization);
    }

    public List<IProduct> GetUserProducts(Guid userID)
    {
        return products.Where(product =>
        {
            if (product.Value is UserProduct userProduct)
                return userProduct.UserID == userID;
            return true;
        }).Select(product => GetProduct(userID, product.Value)).ToList();
    }

    private IProduct GetProduct(Guid userGuid, IProduct product)
    {
        if (product is UserProduct)
            return product;

        if (!sharedProductsCustomizations.ContainsKey(userGuid))
            return product;

        var customization = sharedProductsCustomizations[userGuid]
                .FirstOrDefault(_customization => _customization.ProductGuid == product.Guid);

        return customization == null
            ? product
            : UserProduct.LoadFrom(product, customization);
    }

    public List<IProduct> GetSharedProducts()
    {
        return products
            .Where(product => product.Value is SharedProduct)
            .Select(product => product.Value)
            .ToList();
    }

    public IProduct Save(IProduct product)
    {
        if (products.ContainsKey(product.Guid))
            products[product.Guid] = product;
        else
            products.Add(product.Guid, product);

        return product;
    }

	public void Update(IProduct product, Guid UserId)
	{
		throw new NotImplementedException();
	}
}
