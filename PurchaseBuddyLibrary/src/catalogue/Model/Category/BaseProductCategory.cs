﻿using PurchaseBuddyLibrary.src.catalogue.Model.Product;
using System.Collections.ObjectModel;

namespace PurchaseBuddyLibrary.src.catalogue.Model.Category;

public abstract class BaseProductCategory : IProductCategory
{
	public Guid Guid { get; protected set; }

	public string Name { get; protected set; }
	public string? Description { get; protected set; }

	public IProductCategory? Parent { get; protected set; }
	public Guid? ParentId { get; protected set; }
	public IReadOnlyCollection<IProductCategory> Children => new ReadOnlyCollection<IProductCategory>(children);

	public bool IsRoot { get => !ParentId.HasValue; }

	public List<Guid> GetProductsInCategory()
	{
		if (children == null || !children.Any())
			return productsInCategory;

		var result = new List<Guid>(productsInCategory);
		foreach (var child in children)
		{
			result.AddRange(child.GetProductsInCategory());
		}

		return result;
	}

	public void AddChild(IProductCategory child)
	{
		children.Add(child);
		((BaseProductCategory)child).ParentId = Guid;
		((BaseProductCategory)child).Parent = this;
	}
	public void RemoveChild(IProductCategory child)
	{
		children.Remove(child);
		((BaseProductCategory)child).ParentId = null;
	}
	public abstract void AddProduct(IProduct product);

	public bool ContainsProductWithGuid(Guid userProductId)
	{
		return GetProductsInCategory().Any(productId => productId == userProductId);
	}

	public void RemoveParent()
	{
		ParentId = null;
		Parent = null;
	}

	protected List<Guid> productsInCategory = new List<Guid>();
	protected List<IProductCategory> children = new List<IProductCategory>();
}
