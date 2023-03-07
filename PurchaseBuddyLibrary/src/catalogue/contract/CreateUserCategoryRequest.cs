namespace PurchaseBuddyLibrary.src.catalogue.contract;

public class CreateUserCategoryRequest
{
	public CreateUserCategoryRequest()
	{
	}

	public CreateUserCategoryRequest(string name, string? desc, Guid? parentId)
	{
		Name = name;
		Description = desc;
		ParentId = parentId;
	}
	public string Name { get; set; }
	public string? Description { get; set; }
	public Guid? ParentId { get; set; }
}
