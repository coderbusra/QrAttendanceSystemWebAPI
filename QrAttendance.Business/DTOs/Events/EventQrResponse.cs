namespace QrAttendanceSystem.Business.DTOs.Events;

public class EventQrResponse
{
    public Guid EventId { get; set; }
    public string QrToken { get; set; } = null!;
    public DateTime QrExpire { get; set; }
    public bool IsExpired { get; set; }
}
