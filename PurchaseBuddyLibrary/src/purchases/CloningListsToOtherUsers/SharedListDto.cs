namespace PurchaseBuddyLibrary.src.purchases.CloningListsToOtherUsers;

public record SharedListDto
{
	public DateTime CreatedAt;
	public Guid CreatorId;
	public Guid Guid;
	public Guid SourceId;
	public List<SharedListItemDto> Items;
}

public record SharedListItemDto(string productName, string? categoryName);
