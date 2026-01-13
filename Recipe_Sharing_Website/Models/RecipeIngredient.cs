// Provides validation attributes like [MaxLength]
using System.ComponentModel.DataAnnotations;

// Namespace for model classes
namespace Recipe_Sharing_Website.Models;

// Represents a junction table linking recipes to ingredients
public class RecipeIngredient
{
    // Primary key identity column
    public int RecipeIngredientId { get; set; }

    // Foreign key linking to a recipe
    public int RecipeId { get; set; }

    // Navigation property for the associated recipe
    public Recipe? Recipe { get; set; }

    // Foreign key linking to an ingredient
    public int IngredientId { get; set; }

    // Navigation property for the associated ingredient
    public Ingredient? Ingredient { get; set; }

    // Text quantity (can be "1", "200", "1/2", etc.)
    [MaxLength(30)]
    public string Quantity { get; set; } = "";

    // Unit of measurement (ml, g, tbsp, etc.)
    [MaxLength(20)]
    public string Unit { get; set; } = "";
}
