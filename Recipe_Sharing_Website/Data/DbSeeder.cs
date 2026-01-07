using Recipe_Sharing_Website.Models;
using BCrypt.Net;

namespace Recipe_Sharing_Website.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // -----------------------------
        // Demo User
        // -----------------------------
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "User1",
                Email = "demo@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!")
            });
        }

        // -----------------------------
        // Demo Admin
        // -----------------------------
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

        // -----------------------------
        // Global Ingredients
        // -----------------------------
        if (!db.Ingredients.Any())
        {
            db.Ingredients.AddRange(
                new Ingredient { Name = "Chicken" },
                new Ingredient { Name = "Garlic" },
                new Ingredient { Name = "Rosemary" },
                new Ingredient { Name = "Salt" }
            );
        }

        db.SaveChanges();
    }
}
