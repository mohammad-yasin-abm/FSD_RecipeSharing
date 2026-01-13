// Provides validation attributes such as [Range] and [MaxLength]
using System.ComponentModel.DataAnnotations;

// Namespace for entity models
namespace Recipe_Sharing_Website.Models;

// Represents a recorded payment for premium upgrades
public class Payment
{
    // Primary key identity column
    public int PaymentId { get; set; }

    // Foreign key referencing the User who paid
    public int UserId { get; set; }

    // Navigation property to the related User
    public User? User { get; set; }

    // Payment amount (validated range)
    [Range(0.01, 999999)]
    public decimal Amount { get; set; }

    // Payment method (e.g., card, demo)
    [MaxLength(20)]
    public string Method { get; set; } = "demo";

    // Payment status (success, failed, pending, etc.)
    [MaxLength(20)]
    public string Status { get; set; } = "success";
}
