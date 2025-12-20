using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Ingredient
{
    public int IngredientId { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = "";

    [MaxLength(50)]
    public string Category { get; set; } = "Other";

    // Navigation
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}
