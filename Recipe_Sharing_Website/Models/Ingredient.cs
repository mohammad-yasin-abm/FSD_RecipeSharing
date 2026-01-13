// Data annotation attributes such as [Required] and [MaxLength]
using System.ComponentModel.DataAnnotations;

// Namespace containing core model classes
namespace Recipe_Sharing_Website.Models;

// Represents a single ingredient that can belong to many recipes
public class Ingredient
{
    // Primary key identity column
    public int IngredientId { get; set; }

    // Ingredient name (required, limited to 80 characters)
    [Required, MaxLength(80)]
    public string Name { get; set; } = "";

    // Optional grouping label (e.g. Meat, Dairy, Spice)
    [MaxLength(50)]
    public string Category { get; set; } = "Other";

    // Navigation property representing all recipe links using this ingredient
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}
