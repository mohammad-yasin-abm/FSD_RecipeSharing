using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // -----------------------------
        // Demo User (ensure exists)
        // -----------------------------
        if (!db.Users.Any(u => u.Username == "User1"))
        {
            db.Users.Add(new User
            {
                Username = "User1",
                Email = "demo@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!")
            });
        }

        // -----------------------------
        // Demo Admin (ensure exists)
        // -----------------------------
        if (!db.Admins.Any(a => a.Username == "admin"))
        {
            db.Admins.Add(new Admin
            {
                Username = "admin",
                Email = "admin@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RoleLevel = "admin"
            });
        }

        db.SaveChanges();
    }
}
