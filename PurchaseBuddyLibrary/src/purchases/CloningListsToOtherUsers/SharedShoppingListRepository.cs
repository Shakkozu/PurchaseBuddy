using Dapper;
using Npgsql;
using PurchaseBuddyLibrary.src.utils;

namespace PurchaseBuddyLibrary.src.purchases.CloningListsToOtherUsers;

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
created_at as CreatedAt,
source_list_guid as SourceListGuid,
items
from shared_shopping_lists where guid like @Guid";

		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = connection.QueryFirstOrDefault<SharedShoppingListDao>(sql, new
			{
				Guid = listToShareId.ToDatabaseStringFormat()
			});
			if (dao == null)
				return null;

			return new SharedListDto
			{
				Guid = Guid.Parse(dao.Guid),
				CreatedAt = dao.CreatedAt,
				CreatorId = Guid.Parse(dao.CreatorGuid),
				SourceId = Guid.Parse(dao.SourceListGuid),
				Items = dao.GetShoppingListEntries().Select(x => new SharedListItemDto(x.ProductName, x.CategoryName)).ToList()
			};
		}
	}

	public List<SharedListDto> GetAllWithSourceAndCreator(Guid sourceListGuid, Guid userId)
	{
		const string sql = @"select
id,
guid,
creator_guid as CreatorGuid,
created_at as CreatedAt,
source_list_guid as SourceListGuid,
items
from shared_shopping_lists
where creator_guid like @CreatorGuid and source_list_guid like @SourceListGuid";

		using (var connection = new NpgsqlConnection(connectionString))
		{
			var result = connection.Query<SharedShoppingListDao>(sql, new
			{
				CreatorGuid = userId.ToDatabaseStringFormat(),
				SourceListGuid = sourceListGuid.ToDatabaseStringFormat()
			});
			if (result == null || !result.Any())
				return new List<SharedListDto>();

			return result
				.Select(dao => new SharedListDto
				{
					Guid = Guid.Parse(dao.Guid),
					CreatedAt = dao.CreatedAt,
					CreatorId = Guid.Parse(dao.CreatorGuid),
					SourceId = Guid.Parse(dao.SourceListGuid),
					Items = dao.GetShoppingListEntries().Select(x => new SharedListItemDto(x.ProductName, x.CategoryName)).ToList()
				})
				.ToList();
		}
	}

	public void Save(SharedListDto list)
	{
		const string sql = @"insert into shared_shopping_lists (guid, creator_guid, source_list_guid, created_at, items)
values
(@Guid,
@CreatorGuid,
@SourceListGuid,
@CreatedAt,
@Items)";
		using (var connection = new NpgsqlConnection(connectionString))
		{
			var dao = new SharedShoppingListDao(list);
			connection.ExecuteScalar(sql, new
			{
				dao.Guid,
				dao.CreatorGuid,
				dao.SourceListGuid,
				dao.CreatedAt,
				dao.Items
			});
		}
	}
}
