using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class User
{
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = "";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    public string? ProfilePictureUrl { get; set; }

    // Navigation
    public List<Recipe> Recipes { get; set; } = new();
    public List<Feedback> Feedbacks { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}
