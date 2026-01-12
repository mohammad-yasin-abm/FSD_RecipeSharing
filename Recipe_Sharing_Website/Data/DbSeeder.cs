using Recipe_Sharing_Website.Models; // Import model classes
using BCrypt.Net; // Import BCrypt for password hashing

namespace Recipe_Sharing_Website.Data; // Seeder namespace

public static class DbSeeder // Static seeder class
{
    public static void Seed(AppDbContext db) // Seed method
    {
        if (!db.Users.Any()) // If there are no users
        {
            db.Users.Add(new User // Add a demo user
            {
                Username = "User1", // Demo username
                Email = "demo@recipehub.com", // Demo email
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"), // Demo password hash
                IsPremium = false // Demo user starts as FREE
            });
        }

        if (!db.Admins.Any()) // If there are no admins
        {
            db.Admins.Add(new Admin // Add a demo admin
            {
                Username = "admin", // Admin username
                Email = "admin@recipehub.com", // Admin email
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Admin password hash
                RoleLevel = "admin" // Admin role level
            });
        }

        db.SaveChanges(); // Save changes to database
    }
}
