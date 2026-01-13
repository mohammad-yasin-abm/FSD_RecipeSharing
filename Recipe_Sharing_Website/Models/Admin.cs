// Provides attributes like [Required], [MaxLength], [EmailAddress]
using System.ComponentModel.DataAnnotations;

// Defines entity namespace
namespace Recipe_Sharing_Website.Models;

// Represents an administrator account in the system
public class Admin
{
    // Primary key for the Admin table
    public int AdminId { get; set; }

    // Admin username (required, limited to 50 chars)
    [Required, MaxLength(50)]
    public string Username { get; set; } = "";

    // Admin email address (required + validated format)
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";

    // Hashed password for secure storage
    [Required]
    public string PasswordHash { get; set; } = "";

    // Optional string defining permission level
    [MaxLength(30)]
    public string RoleLevel { get; set; } = "moderator";
}
