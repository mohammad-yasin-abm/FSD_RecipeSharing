// Enables model validation attributes such as [Required]
using System.ComponentModel.DataAnnotations;

// Namespace for entity models
namespace Recipe_Sharing_Website.Models;

// Represents a recipe posted by a user
public class Recipe
{
    // Primary key identity column
    public int RecipeId { get; set; }

    // Title of the recipe (required)
    [Required]
    public string Title { get; set; } = "";

    // Optional short description summarizing the recipe
    public string? Description { get; set; }

    // Free-text listing of ingredients (unstructured)
    public string? IngredientsText { get; set; }

    // Free-text instructions or cooking steps
    public string? StepsText { get; set; }

    // Foreign key identifying recipe owner
    public int UserId { get; set; }

    // Navigation property to the recipe's owner
    public User? User { get; set; }

    // Collection of comments/ratings tied to this recipe
    public List<Feedback> Feedbacks { get; set; } = new();
}
