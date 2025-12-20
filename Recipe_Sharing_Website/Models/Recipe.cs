using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Recipe
{
    public int RecipeId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    // Keep steps as text for simplicity
    public string StepsText { get; set; } = "";

    [MaxLength(50)]
    public string Category { get; set; } = "General";

    public int CookTimeMinutes { get; set; }

    // Navigation
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    public List<Feedback> Feedbacks { get; set; } = new();
}
