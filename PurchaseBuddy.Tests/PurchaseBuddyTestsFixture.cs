using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using System.Transactions;

namespace PurchaseBuddy.Tests;
internal class PurchaseBuddyTestsFixture
{
	protected TransactionScope _transactionScope;

	protected UserProductCategory AUserProductCategory(string? name = null)
	{
		return UserProductCategory.CreateNew(name ?? "dairy", UserId);
	}
	protected CreateUserCategoryRequest AUserProductCategoryCreateRequest(string? name = null, Guid? parentId = null, string? description = null)
	{
		return new CreateUserCategoryRequest(name ?? "dairy", description, parentId);
	}

	protected SharedProductCategory ASharedCategory(string name, IProductCategory? parent = null)
	{
		return parent != null
			? SharedProductCategory.CreateNewWithParent(name, parent)
			: SharedProductCategory.CreateNew(name);
	}

	protected UserProductCategory AUserProductCategoryWithParent(UserProductCategory parent, string? name = null)
	{
		var child = UserProductCategory.CreateNewWithParent(name ?? "eggs", UserId, parent);
		parent.AddChild(child);

		return child;
	}

	protected Guid AUserCreated()
	{
		var userRepository = new UserRepository(TestConfigurationHelper.GetConnectionString());
		var authService = new AuthorizationService(userRepository, null);
		UserId = authService.Register(new UserDto { Password = "examplePassword123!", Login = "exampleLogin123", Email = "test@example.com" });
		return UserId;
	}
    protected Guid ANewUserCreated()
	{
		var userRepository = new UserRepository(TestConfigurationHelper.GetConnectionString());
		var authService = new AuthorizationService(userRepository, null);

		return authService.Register(new UserDto { Password = "examplePassword123!", Login = Guid.NewGuid().ToString(), Email = $"{Guid.NewGuid()}@example.com" });
	}

	[SetUp]
	public void SetUp()
	{
		_transactionScope = new TransactionScope();
	}

	[TearDown]
	public void TearDown()
	{
		_transactionScope.Dispose();
	}

	protected Guid UserId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
}
