using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public SubscriptionType Type { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? CancelledAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    [Required]
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum InvoiceStatus
{
    Pending = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4
}

public class CreateSubscriptionRequest
{
    [Required]
    public SubscriptionType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class RenewSubscriptionRequest
{
    public DateTime? NewExpiresAt { get; set; }
}
