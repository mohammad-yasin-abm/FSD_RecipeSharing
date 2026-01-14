using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Recipe
{
    [Key]
    public int RecipeId { get; set; }

    public int UserId { get; set; } // Owner user id

    [Required]
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    // Free-text ingredients (ONLY ONE PROPERTY - do NOT duplicate)
    public string IngredientsText { get; set; } = "";

    // Free-text steps (ONLY ONE PROPERTY - do NOT duplicate)
    public string StepsText { get; set; } = "";

    // Navigation (optional)
    public List<Feedback> Feedbacks { get; set; } = new();
}
