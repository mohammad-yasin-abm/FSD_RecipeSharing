using Microsoft.EntityFrameworkCore;
using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Payment> Payments => Set<Payment>();
}
