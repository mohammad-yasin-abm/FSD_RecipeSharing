using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Recipe_Sharing_Website.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; } = "";

    public string? Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } = "";

    // ✅ Navigation: needed because AppDbContext references User.Feedbacks
    public List<Feedback> Feedbacks { get; set; } = new();
}
