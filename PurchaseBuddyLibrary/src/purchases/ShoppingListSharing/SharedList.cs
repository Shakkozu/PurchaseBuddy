namespace PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

internal class SharedList
{
	public Guid CreatorId { get; }
	public Guid Guid { get; }
	public Guid SourceListGuid { get; }

	internal SharedList(Guid creatorId, List<SharedListItem> listItems, Guid listId)
	{
		CreatorId = creatorId;
		items = listItems;
		createdAt = DateTime.Now;
		SourceListGuid = listId;

		Guid = Guid.NewGuid();
	}

	internal SharedListDto ToDto()
	{
		return new SharedListDto
		{
			CreatedAt = createdAt,
			CreatorId = CreatorId,
			SourceId = SourceListGuid,
			Guid = Guid,
			Items = items.Select(item => item.ToDto()).ToList()
		};
	}

	private readonly DateTime createdAt;
	private readonly List<SharedListItem> items;
}

internal record SharedListItem(string productName, string? categoryName)
{
	public SharedListItemDto ToDto()
	{
		return new SharedListItemDto(productName, categoryName);
	}
}
