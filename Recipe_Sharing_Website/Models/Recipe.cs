using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Recipe
{
    public int RecipeId { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    // Free-text
    public string? IngredientsText { get; set; }

    // ✅ NEW: Free-text steps
    public string? StepsText { get; set; }

    // Owner
    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Feedback> Feedbacks { get; set; } = new();
}
