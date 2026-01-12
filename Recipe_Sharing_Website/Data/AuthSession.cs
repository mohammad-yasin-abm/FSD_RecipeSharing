using Microsoft.AspNetCore.Http;

namespace Recipe_Sharing_Website.Data;

public static class AuthSession
{
    public const string RoleKey = "Auth_Role";        // "User" or "Admin"
    public const string UserIdKey = "Auth_UserId";    // user id as string
    public const string AdminIdKey = "Auth_AdminId";  // admin id as string
    public const string NameKey = "Auth_Name";        // username
    public const string PremiumKey = "Auth_Premium";  // "true" or "false"

    public static bool IsLoggedIn(ISession session) =>
        !string.IsNullOrWhiteSpace(session.GetString(RoleKey));

    public static bool IsAdmin(ISession session) =>
        session.GetString(RoleKey) == "Admin";

    public static bool IsPremium(ISession session) =>
        session.GetString(PremiumKey) == "true";

    public static int? GetUserId(ISession session)
    {
        var s = session.GetString(UserIdKey);
        return int.TryParse(s, out var id) ? id : null;
    }

    public static string? GetUsername(ISession session) =>
        session.GetString(NameKey);

    public static void Logout(ISession session) => session.Clear();
}
