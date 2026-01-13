// EF Core and database engine APIs
using Microsoft.EntityFrameworkCore;

// Access to entity classes
using Recipe_Sharing_Website.Models;

// Namespace for the data layer
namespace Recipe_Sharing_Website.Data;

// Main EF Core DbContext for the application
public class AppDbContext : DbContext
{
    // Constructor receives DbContextOptions from DI
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tables in the database
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Configure entity relationships and indexes
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevent duplicate ingredient assignments per recipe
        modelBuilder.Entity<RecipeIngredient>()
            .HasIndex(x => new { x.RecipeId, x.IngredientId })
            .IsUnique();

        // Configure Feedback -> User relationship to avoid multiple cascade paths
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure cascade delete from Recipe -> Feedback
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Recipe)
            .WithMany(r => r.Feedbacks)
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set decimal precision for Payment.Amount
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);
    }
}
