using Microsoft.EntityFrameworkCore;
using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1) Junction table: prevent duplicate (RecipeId, IngredientId)
        modelBuilder.Entity<RecipeIngredient>()
            .HasIndex(x => new { x.RecipeId, x.IngredientId })
            .IsUnique();

        // 2) Fix SQL Server "multiple cascade paths" for Feedback
        // Keep cascade from Recipe -> Feedback, but NOT from User -> Feedback
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // (Optional but recommended) Ensure cascade from Recipe -> Feedback
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Recipe)
            .WithMany(r => r.Feedbacks)
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3) Avoid decimal truncation warning for Payment.Amount
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);
    }
}
