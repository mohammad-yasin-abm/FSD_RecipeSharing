using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    public int IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    // “200”, “2”, “1/2” etc.
    [MaxLength(30)]
    public string Quantity { get; set; } = "";

    // “g”, “ml”, “tbsp”, “cup”
    [MaxLength(20)]
    public string Unit { get; set; } = "";
}
