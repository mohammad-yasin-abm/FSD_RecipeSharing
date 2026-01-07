using Microsoft.AspNetCore.Mvc;
using Recipe_Sharing_Website.Data;
using Recipe_Sharing_Website.Models;

namespace Recipe_Sharing_Website.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("create")]
    public IActionResult CreatePayment([FromBody] PaymentRequest req)
    {
        if (req.Amount <= 0)
        {
            return BadRequest("Invalid amount.");
        }

        var payment = new Payment
        {
            UserId = req.UserId,
            Amount = req.Amount,
            Status = "Success"
        };

        _db.Payments.Add(payment);
        _db.SaveChanges();

        return Ok(new
        {
            message = "Payment successful (demo)",
            paymentId = payment.PaymentId
        });
    }
}

public class PaymentRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}
