using Microsoft.AspNetCore.Http; // ISession extensions

namespace Recipe_Sharing_Website.Data; // Same namespace as your project

public static class AuthSession // Session auth helper
{
    public const string RoleKey = "Role"; // "User" or "Admin"
    public const string UserIdKey = "UserId"; // user id stored as string
    public const string AdminIdKey = "AdminId"; // admin id stored as string
    public const string NameKey = "Name"; // username
    public const string PremiumKey = "IsPremium"; // "true"/"false"

    public static bool IsLoggedIn(ISession session) // logged in if role exists
    {
        return !string.IsNullOrWhiteSpace(session.GetString(RoleKey)); // check role
    }

    public static bool IsAdmin(ISession session) // admin if role is Admin
    {
        return session.GetString(RoleKey)?.ToLower() == "admin"; // compare string
    }

    public static int? GetUserId(ISession session) // read user id
    {
        var raw = session.GetString(UserIdKey); // get stored string
        if (int.TryParse(raw, out var id)) return id; // parse to int
        return null; // return null if missing
    }

    public static int? GetAdminId(ISession session) // read admin id
    {
        var raw = session.GetString(AdminIdKey); // get stored string
        if (int.TryParse(raw, out var id)) return id; // parse to int
        return null; // return null if missing
    }

    public static bool IsPremium(ISession session) // read premium flag
    {
        return session.GetString(PremiumKey) == "true"; // stored as "true"
    }

    public static void SetPremium(ISession session, bool isPremium) // set premium flag
    {
        session.SetString(PremiumKey, isPremium ? "true" : "false"); // store as string
    }

    public static void Logout(ISession session) // clear auth session
    {
        session.Clear(); // clears everything
    }
}
