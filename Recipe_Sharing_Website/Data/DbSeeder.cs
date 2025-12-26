using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Demo user
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "demo_user",
                Email = "demo@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!")
            });
            db.SaveChanges();
        }

        // Demo admin
        if (!db.Admins.Any())
        {
            db.Admins.Add(new Admin
            {
                Username = "admin",
                Email = "admin@recipehub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RoleLevel = "admin"
            });
            db.SaveChanges();
        }

        // Ingredients
        if (!db.Ingredients.Any())
        {
            db.Ingredients.AddRange(
                new Ingredient { Name = "Chicken", Category = "Meat" },
                new Ingredient { Name = "Garlic", Category = "Spice" },
                new Ingredient { Name = "Rosemary", Category = "Herb" },
                new Ingredient { Name = "Salt", Category = "Seasoning" }
            );
            db.SaveChanges();
        }
    }
}
