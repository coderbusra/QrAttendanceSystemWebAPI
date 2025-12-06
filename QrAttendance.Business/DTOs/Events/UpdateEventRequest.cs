namespace QrAttendanceSystem.Business.DTOs.Events;

public class UpdateEventRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public DateTime QrExpire { get; set; }
}
