// Provides attributes like ApiController, ControllerBase, Route
using Microsoft.AspNetCore.Mvc;

// Allows async LINQ queries and EF database operations
using Microsoft.EntityFrameworkCore;

// Provides access to AppDbContext and session helpers
using Recipe_Sharing_Website.Data;

// Provides User and Payment entity models
using Recipe_Sharing_Website.Models;

// Namespace where this controller resides
namespace Recipe_Sharing_Website.Controllers;

// Indicates this is an API controller
[ApiController]

// Base route prefix for all endpoints under this controller
[Route("api/payments")]

// Controller class definition
public class PaymentsController : ControllerBase
{
    // Factory for creating DB contexts safely per request
    private readonly IDbContextFactory<AppDbContext> _factory;

    // Constructor injects the factory from DI
    public PaymentsController(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    // Simple debugging endpoint to verify controller is reachable
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Payments API is alive");
    }

    // Create a new payment and mark a user as premium
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest req)
    {
        // Basic validation checks
        if (req.UserId <= 0) return BadRequest("Invalid userId.");
        if (req.Amount <= 0) return BadRequest("Invalid amount.");

        // Create unique DB context instance
        await using var db = await _factory.CreateDbContextAsync();

        // Retrieve the user from the database
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);
        if (user is null) return NotFound("User not found.");

        // Build the payment object
        var payment = new Payment
        {
            UserId = req.UserId,
            Amount = req.Amount,
            Status = "Success"
        };

        // Add payment and upgrade user
        db.Payments.Add(payment);
        user.IsPremium = true;

        // Save changes to DB
        await db.SaveChangesAsync();

        // Return success response with details
        return Ok(new
        {
            message = "Payment successful (demo). User upgraded to Premium.",
            paymentId = payment.PaymentId,
            userId = user.UserId,
            username = user.Username,
            isPremium = user.IsPremium
        });
    }

    // Retrieve all users marked as premium
    [HttpGet("premium-users")]
    [HttpGet("premiumusers")]
    public async Task<IActionResult> GetPremiumUsers()
    {
        // Open DB context
        await using var db = await _factory.CreateDbContextAsync();

        // Query minimal user info for premium accounts
        var premiumUsers = await db.Users
            .Where(u => u.IsPremium)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.IsPremium
            })
            .ToListAsync();

        return Ok(premiumUsers);
    }
}

// DTO used for creating payments
public class PaymentRequest
{
    // User making the payment
    public int UserId { get; set; }

    // Amount being submitted
    public decimal Amount { get; set; }
}
