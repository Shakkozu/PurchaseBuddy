namespace PurchaseBuddyLibrary.src.auth.app;

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

public class SessionExpiredException : Exception
{
	public SessionExpiredException(Guid sessionId) : base($"session {sessionId} has expired")
	{
	}
}
public class Session
{

	public Guid SessionId { get; }
	public Guid UserId { get; }
	public bool IsExpired => expiresAt < DateTime.Now;

	public static Session CreateNew(Guid userId, int ttlInMinutes)
	{
		return new Session(Guid.NewGuid(), userId, DateTime.Now.AddMinutes(ttlInMinutes));
	}

	private Session(Guid sessionId, Guid userId, DateTime expiresAt)
	{
		SessionId = sessionId;
		UserId = userId;
		this.expiresAt = expiresAt;
	}

	private readonly DateTime expiresAt;
}