using System.ComponentModel.DataAnnotations;

namespace HireConnect.NotificationService.DTOs;

public class CreateNotificationDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "Type is required")]
    [MaxLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
    public string Type { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Message is required")]
    [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; set; } = string.Empty;
}

public class NotificationResponseDto
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
