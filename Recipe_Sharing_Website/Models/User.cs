// Namespace for application model classes
namespace Recipe_Sharing_Website.Models;

// Represents an account for a regular user
public class User
{
    // Primary key identity column
    public int UserId { get; set; }

    // Display name chosen by the user
    public string Username { get; set; } = "";

    // Email address used for login or contact
    public string Email { get; set; } = "";

    // Securely stored (hashed) password
    public string PasswordHash { get; set; } = "";

    // Premium tier flag (true if upgraded)
    public bool IsPremium { get; set; } = false;
}
