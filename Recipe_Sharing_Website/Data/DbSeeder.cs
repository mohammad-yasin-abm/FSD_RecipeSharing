// Provides access to User, Admin models
using Recipe_Sharing_Website.Models;

// Provides BCrypt for secure password hashing
using BCrypt.Net;

// Namespace for seeding utilities
namespace Recipe_Sharing_Website.Data;

// Static class responsible for seeding default data
public static class DbSeeder
{
    // Executes initial seeding if database is empty
    public static void Seed(AppDbContext db)
    {
        // Add default demo user if users table is empty
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "User1",
                Email = "demo@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                IsPremium = false
            });
        }

        // Add default admin if admins table is empty
        if (!db.Admins.Any())
        {
            db.Admins.Add(new Admin
            {
                Username = "admin",
                Email = "admin@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RoleLevel = "admin"
            });
        }

        // Commit all inserted entities to the database
        db.SaveChanges();
    }
}
