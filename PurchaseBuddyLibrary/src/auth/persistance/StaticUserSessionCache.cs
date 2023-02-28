using PurchaseBuddyLibrary.src.auth.model;

namespace PurchaseBuddyLibrary.src.auth.persistance;

public static class StaticUserSessionCache
{
	public static void Add(Session session)
	{
		cache.Add(session.SessionId, session);
	}

	public static Session? FindByUserId(Guid userId)
	{
		cache.TryGetValue(userId, out var value);

		return value;
	}

	public static Session? Load(Guid sessionId)
	{
		return cache.TryGetValue(sessionId, out var value) ? value : null;
	}

	public static void Delete(Session session)
	{
		if (cache.ContainsKey(session.SessionId))
			cache.Remove(session.SessionId);
	}

	private static Dictionary<Guid, Session> cache = new();
}
