using Dapper;
using Npgsql;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.ShoppingListSharing;

public class SharedShoppingListRepository : ISharedShoppingListRepository
{
	private readonly string connectionString;

	public SharedShoppingListRepository(string connectionString)
    {
		this.connectionString = connectionString;
	}
    public SharedListDto? Get(Guid listToShareId)
	{
        const string sql = @"select
id,
guid,
creator_guid as CreatorGuid,
source_list_guid as SourceListGuid,
items
from shared_shopping_lists where guid like @Guid";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            var dao = connection.Query<SharedShoppingListDao>(sql, new
            {
                Guid = listToShareId.ToDatabaseStringFormat()
            });
            if (dao == null)
                return null;

            return new SharedListDto(dao);
        }
	}

	public List<SharedListDto> GetAllWithSourceAndCreator(Guid listId, Guid userId)
	{
		throw new NotImplementedException();
	}

	public void Save(SharedListDto list)
	{
		throw new NotImplementedException();
	}
}

internal class SharedShoppingListDao
{
    public int Id { get; set; }
    public string Guid { get; set; }
    public string CreatorGuid { get; set; }
    public string SourceListGuid { get; set; }
    public string Items { get; set; }
}

public class InMemorySharedShoppingListRepository : ISharedShoppingListRepository
{
    private Dictionary<Guid, SharedListDto> _cache = new Dictionary<Guid, SharedListDto>();

    public void Save(SharedListDto list)
    {
        _cache[list.Guid] = list;
    }

    public SharedListDto? Get(Guid listToShareId)
    {
        return _cache.TryGetValue(listToShareId, out var value) ? value : null;
    }

    public List<SharedListDto> GetAllWithSourceAndCreator(Guid listId, Guid userId)
    {
        return _cache.Values.Where(list => list.CreatorId == userId && list.SourceId == listId).ToList();
    }
}