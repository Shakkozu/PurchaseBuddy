namespace PurchaseBuddyLibrary.src.utils;

public interface ITimeService
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}