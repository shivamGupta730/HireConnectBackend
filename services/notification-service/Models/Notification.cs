using System.ComponentModel.DataAnnotations;

namespace HireConnect.NotificationService.Models;

public class Notification
{
    public int NotificationId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
