using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Product;

namespace PurchaseBuddyLibrary.src.catalogue.Commands.SharedProducts;
public class AddNewSharedProductCommand
{
    public string Name { get; set; }
    public Guid? CategoryGuid { get; set; }
    public Guid UserGuid { get; set; }
}

public class AddNewSharedProductCommandHandler
{
    private readonly ISharedProductRepository productsRepository;
    private readonly IUserRepository userRepository;

    public AddNewSharedProductCommandHandler(ISharedProductRepository productsRepository, IUserRepository userRepository)
    {
        this.productsRepository = productsRepository;
        this.userRepository = userRepository;
    }

    public Guid Handle(AddNewSharedProductCommand command)
    {
        if (!CanHandle(command))
            throw new InvalidOperationException();

        var product = SharedProduct.CreateNew(command.Name);
        productsRepository.Save(product);

        return product.Guid;
    }

    public bool CanHandle(AddNewSharedProductCommand command)
    {
        var user = userRepository.GetByGuid(command.UserGuid);

        return user.IsAdministrator;
    }
}