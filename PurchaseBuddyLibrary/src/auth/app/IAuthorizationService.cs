using PurchaseBuddyLibrary.src.auth.contract;
using PurchaseBuddyLibrary.src.auth.model;

namespace PurchaseBuddyLibrary.src.auth.app;
public interface IUserAuthorizationService
{
    Guid Register(RegisterUserRequest userDto);
    Guid Login(string login, string password);

    User GetUserFromSessionId(Guid sessionId);
    UserSessionInfo GetUserSessionInfo(Guid sessionId);
    void Logout(Guid sessionId);
}