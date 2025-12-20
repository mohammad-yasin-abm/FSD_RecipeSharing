using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Admin
{
    public int AdminId { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = "";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [MaxLength(30)]
    public string RoleLevel { get; set; } = "moderator";
}
