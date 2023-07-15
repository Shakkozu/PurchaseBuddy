namespace PurchaseBuddyLibrary.src.utils;

public class TimeService : ITimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}