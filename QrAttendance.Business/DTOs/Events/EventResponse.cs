namespace QrAttendanceSystem.Business.DTOs.Events;

public class EventResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime Date { get; set; }
    public DateTime QrExpire { get; set; }

    // 🔹 Front-end’in kullanacağı token
    public string QrToken { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = null!;

    public bool IsQrExpired { get; set; }
}