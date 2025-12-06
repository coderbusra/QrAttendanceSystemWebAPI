namespace QrAttendanceSystem.Business.DTOs.Events;

public class CreateEventRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    /// <summary> Etkinliğin tarih/saat bilgisi </summary>
    public DateTime Date { get; set; }

    /// <summary> QR kodunun geçerli olacağı son zaman </summary>
    public DateTime QrExpire { get; set; }
}
