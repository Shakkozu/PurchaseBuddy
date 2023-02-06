namespace PurchaseBuddyLibrary.src.auth.app;


public interface IUserSessionCache
{
	void Add(Session session);
	void Delete(Session session);
	Session? Load(Guid sessionId);
}
public static class StaticUserSessionCache
{
	public static void Add(Session session)
	{
		cache.Add(session.SessionId, session);
	}

	public static Session? Find(Guid sessionId)
	{
		if (cache.ContainsKey(sessionId))
			return cache[sessionId];

		return null;
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
public class UserSessionCache : IUserSessionCache
{
	// initalize event generated every 2h, that will clear all expired sessions
	public UserSessionCache()
	{
		var timer = new System.Timers.Timer(TimeSpan.FromHours(2).TotalMilliseconds);
		timer.Elapsed += (sender, args) => cache = cache
			.Where(x => !x.Value.IsExpired)
			.ToDictionary(x => x.Key, x => x.Value);

		timer.Start();
	}
	public void Add(Session session)
	{
		cache.Add(session.SessionId, session);
	}

	public Session? Find(Guid sessionId)
	{
		if (cache.ContainsKey(sessionId))
			return cache[sessionId];

		return null;
	}

	public Session? Load(Guid sessionId)
	{
		return cache.TryGetValue(sessionId, out var value) ? value : null;
	}

	public void Delete(Session session)
	{
		if (cache.ContainsKey(session.SessionId))
			cache.Remove(session.SessionId);
	}

	private Dictionary<Guid, Session> cache = new();
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