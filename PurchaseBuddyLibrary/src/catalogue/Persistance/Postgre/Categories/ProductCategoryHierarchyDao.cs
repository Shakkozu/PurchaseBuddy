﻿namespace PurchaseBuddyLibrary.src.catalogue.Persistance.Postgre.Categories;

public class ProductCategoryHierarchyDao
{
    public int Id { get; set; }
    public string UserGuid { get; set; }
    public string CategoryGuid { get; set; }
    public string RootPath { get; set; }
}
