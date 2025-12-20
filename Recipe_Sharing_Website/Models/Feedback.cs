using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Feedback
{
    public int FeedbackId { get; set; }
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(500)]
    public string Message { get; set; } = "";

    // optional rating 1–5
    [Range(1, 5)]
    public int? Rating { get; set; }
}
