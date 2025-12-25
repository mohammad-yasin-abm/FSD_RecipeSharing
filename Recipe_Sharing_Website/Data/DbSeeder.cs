using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Seed a demo user if none exists
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "demo_user",
                Email = "demo@recipehub.com",
                PasswordHash = "demo_hash"
            });

            db.SaveChanges();
        }

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
