using Microsoft.AspNetCore.Mvc; // ControllerBase + routing attributes
using Microsoft.EntityFrameworkCore; // EF Core
using Recipe_Sharing_Website.Data; // AppDbContext
using Recipe_Sharing_Website.Models; // User + Payment models

namespace Recipe_Sharing_Website.Controllers; // Controller namespace

[ApiController] // API conventions
[Route("api/payments")] // Base route
public class PaymentsController : ControllerBase // Controller
{
    private readonly IDbContextFactory<AppDbContext> _factory; // DbContextFactory

    public PaymentsController(IDbContextFactory<AppDbContext> factory) // Inject factory
    {
        _factory = factory; // Store factory
    }

    [HttpGet("ping")] // GET api/payments/ping
    public IActionResult Ping() // Health check
    {
        return Ok("Payments API is alive"); // Response
    }

    [HttpPost("create")] // POST api/payments/create
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest req) // Create payment
    {
        if (req.UserId <= 0) return BadRequest("Invalid userId."); // Validate userId
        if (req.Amount <= 0) return BadRequest("Invalid amount."); // Validate amount

        await using var db = await _factory.CreateDbContextAsync(); // Create DbContext

        var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId); // Find user
        if (user is null) return NotFound("User not found."); // 404 if missing

        var payment = new Payment // Create payment record
        {
            UserId = req.UserId, // Set userId
            Amount = req.Amount, // Set amount
            Status = "Success" // Demo success
        };

        db.Payments.Add(payment); // Add payment
        user.IsPremium = true; // Upgrade user
        await db.SaveChangesAsync(); // Save

        return Ok(new // Return JSON
        {
            message = "Payment successful (demo). User upgraded to Premium.",
            paymentId = payment.PaymentId,
            userId = user.UserId,
            username = user.Username,
            isPremium = user.IsPremium
        });
    }

    [HttpGet("premium-users")] // GET api/payments/premium-users
    [HttpGet("premiumusers")] // GET api/payments/premiumusers (alias)
    public async Task<IActionResult> GetPremiumUsers() // List premium users
    {
        await using var db = await _factory.CreateDbContextAsync(); // Create DbContext

        var premiumUsers = await db.Users // Query users
            .Where(u => u.IsPremium) // Only premium
            .Select(u => new // Return minimal fields
            {
                u.UserId,
                u.Username,
                u.Email,
                u.IsPremium
            })
            .ToListAsync(); // Execute query

        return Ok(premiumUsers); // Return list
    }
}

public class PaymentRequest // DTO
{
    public int UserId { get; set; } // User id
    public decimal Amount { get; set; } // Amount
}
