﻿using PurchaseBuddy.src.catalogue.Persistance;
using PurchaseBuddyLibrary.src.catalogue.Model.Category;
using PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Products;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Product;
public class UserProduct : IProduct
{
	public int Id { get; }
	public Guid Guid { get; }
	public Guid UserID { get; }
	public Guid? CategoryId { get; private set; }
	public string Name { get; set; }
	public static UserProduct Create(string name, Guid userId, Guid? categoryId = null)
	{
		return new UserProduct(null, userId, name, Guid.NewGuid(), categoryId);
	}
	public void AssignProductToCategory(IProductCategory category)
	{
		CategoryId = category.Guid;
	}

	internal static IProduct LoadFrom(IProduct product, SharedProductCustomization customization)
	{
		return new UserProduct(product.Id, customization.UserID, customization.Name, product.Guid, customization.CategoryId);
	}
	internal static IProduct LoadFrom(ProductDao result)
	{
		return new UserProduct(result.Id,
			Guid.Parse(result.UserGuid),
			result.Name,
			Guid.Parse(result.Guid),
			string.IsNullOrEmpty(result.CategoryGuid) ? null : Guid.Parse(result.CategoryGuid));
	}

	public void RemoveProductCategory()
	{
		CategoryId = null;
	}


	private UserProduct(int? id, Guid userID, string name, Guid guid, Guid? categoryId)
	{
		if (id.HasValue)
			Id = id.Value;

		UserID = userID;
		Name = name;
		Guid = guid;
		CategoryId = categoryId;
	}
}
