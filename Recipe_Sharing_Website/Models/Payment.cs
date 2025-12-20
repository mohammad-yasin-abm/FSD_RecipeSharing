using System.ComponentModel.DataAnnotations;

namespace Recipe_Sharing_Website.Models;

public class Payment
{
    public int PaymentId { get; set; }

    // FK → User
    public int UserId { get; set; }
    public User? User { get; set; }

    [Range(0.01, 999999)]
    public decimal Amount { get; set; }

    [MaxLength(20)]
    public string Method { get; set; } = "demo";

    [MaxLength(20)]
    public string Status { get; set; } = "success"; // success/failed/pending
}
