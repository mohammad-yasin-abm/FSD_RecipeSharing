// FILE: Data/AuthSession.cs
using Microsoft.AspNetCore.Http;

namespace Recipe_Sharing_Website.Data;

public static class AuthSession
{
    public const string RoleKey = "Auth_Role";   // "User" or "Admin"
    public const string UserIdKey = "Auth_UserId";
    public const string AdminIdKey = "Auth_AdminId";
    public const string NameKey = "Auth_Name";

    public static bool IsLoggedIn(ISession session) =>
        !string.IsNullOrWhiteSpace(session.GetString(RoleKey));

    public static bool IsAdmin(ISession session) =>
        session.GetString(RoleKey) == "Admin";

    public static int? GetUserId(ISession session)
    {
        var raw = session.GetString(UserIdKey);
        return int.TryParse(raw, out var id) ? id : null;
    }

    public static int? GetAdminId(ISession session)
    {
        var raw = session.GetString(AdminIdKey);
        return int.TryParse(raw, out var id) ? id : null;
    }

    public static string? GetName(ISession session) =>
        session.GetString(NameKey);

    public static void Logout(ISession session) => session.Clear();
}
