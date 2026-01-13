// Provides [Required] and model validation attributes
using System.ComponentModel.DataAnnotations;

// Namespace for entity models
namespace Recipe_Sharing_Website.Models;

// Stores user feedback and optional ratings for a recipe
public class Feedback
{
    // Primary key identity column
    public int FeedbackId { get; set; }

    // Foreign key linking feedback to a recipe
    [Required]
    public int RecipeId { get; set; }

    // Navigation property to parent recipe
    public Recipe? Recipe { get; set; }

    // Foreign key linking feedback to a user
    [Required]
    public int UserId { get; set; }

    // Navigation property to User
    public User? User { get; set; }

    // Optional rating value (1–5), nullable for flexibility
    public int? Rating { get; set; }

    // Optional written comment from user
    public string? Message { get; set; }

    // Timestamp when feedback was created (UTC)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
