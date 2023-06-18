using System.Runtime.Serialization;

namespace PurchaseBuddyLibrary.src.auth.model;

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