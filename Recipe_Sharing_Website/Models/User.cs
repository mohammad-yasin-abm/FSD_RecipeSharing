namespace Recipe_Sharing_Website.Models; // Define the namespace for model classes

public class User // Define the User entity
{
    public int UserId { get; set; } // Primary key for the user

    public string Username { get; set; } = ""; // Username

    public string Email { get; set; } = ""; // Email address

    public string PasswordHash { get; set; } = ""; // BCrypt password hash

    public bool IsPremium { get; set; } = false; // Premium flag (false = free tier)
}
