// For accessing session on HTTP context
using Microsoft.AspNetCore.Http;

// Namespace for shared session helpers
namespace Recipe_Sharing_Website.Data;

// Static helper class to read/write authentication values in session
public static class AuthSession
{
    // Session key used to store user/admin role
    public const string RoleKey = "Auth_Role";

    // Session key used to store logged-in user id
    public const string UserIdKey = "Auth_UserId";

    // Session key used to store logged-in admin id
    public const string AdminIdKey = "Auth_AdminId";

    // Session key used to store username
    public const string NameKey = "Auth_Name";

    // Session key used to track premium membership
    public const string PremiumKey = "Auth_Premium";

    // True if session contains a role (user is logged in)
    public static bool IsLoggedIn(ISession session) =>
        !string.IsNullOrWhiteSpace(session.GetString(RoleKey));

    // True if the session role is admin
    public static bool IsAdmin(ISession session) =>
        session.GetString(RoleKey) == "Admin";

    // True if session explicitly stores premium = "true"
    public static bool IsPremium(ISession session) =>
        session.GetString(PremiumKey) == "true";

    // Reads the user id from session and converts to int?
    public static int? GetUserId(ISession session)
    {
        var s = session.GetString(UserIdKey);
        return int.TryParse(s, out var id) ? id : null;
    }

    // Retrieves stored username if any
    public static string? GetUsername(ISession session) =>
        session.GetString(NameKey);

    // Clears all stored session values (logout)
    public static void Logout(ISession session) =>
        session.Clear();
}
