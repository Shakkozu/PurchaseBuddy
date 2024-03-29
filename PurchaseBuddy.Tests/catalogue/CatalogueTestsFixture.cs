﻿using Dapper;
using Npgsql;
using NUnit.Framework.Internal;
using PurchaseBuddyLibrary.src.auth.app;
using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.persistance;
using PurchaseBuddyLibrary.src.catalogue.contract;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;

namespace PurchaseBuddy.Tests.catalogue;

internal class CatalogueTestsFixture
{
	protected UserProductCategory AUserProductCategory(string? name = null)
	{
		return UserProductCategory.CreateNew(name ?? "dairy", UserId);
	}
	protected CreateUserCategoryRequest AUserProductCategoryCreateRequest(string? name = null, Guid? parentId = null, string? description = null)
	{
		return new CreateUserCategoryRequest(name ?? "dairy", description, parentId);
	}

	public SharedProductCategory ASharedCategory(string name, IProductCategory? parent = null)
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


	public virtual void TearDown()
	{
		using (var connection = new NpgsqlConnection(TestConfigurationHelper.GetConnectionString()))
		{
			connection.Execute("delete from shopping_invitations");
			connection.Execute("delete from shared_shopping_lists");
			connection.Execute("delete from shared_products_customization");
			connection.Execute("delete from user_products");
			connection.Execute("delete from shared_products");
			connection.Execute("delete from product_categories_hierarchy");
			connection.Execute("delete from product_categories");
			connection.Execute("delete from shopping_lists");
			connection.Execute("delete from shops");
			connection.Execute("delete from users");
			connection.Dispose();
		}
	}
	protected Guid AUserCreated(string login = "testUser")
	{
		var userRepository = new UserRepository(TestConfigurationHelper.GetConnectionString());
		var authService = new AuthorizationService(userRepository, null);

		return authService.Register(new RegisterUserRequest { Password = "examplePassword123!", ConfirmPassword = "examplePassword123!",Login = login, Email = login + "@example.com"});
	}
	
	protected Guid AdministratorCreated()
	{
		var userRepository = new UserRepository(TestConfigurationHelper.GetConnectionString());
		var authService = new AuthorizationService(userRepository, null);
		UserId = authService.Register(new RegisterUserRequest { Password = "examplePassword123!", Login = "admin", Email = "admin@example.com" });
		authService.GrantAdministratorAccessRights(UserId);
		return UserId;
	}

    protected Guid UserId = Guid.Parse("8FFEE1B4-ADDF-4C5A-B773-16C4830FC278");
}