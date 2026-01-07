using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Feedback
{
    public int FeedbackId { get; set; }

    [Required]
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }

    public int? Rating { get; set; }
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
