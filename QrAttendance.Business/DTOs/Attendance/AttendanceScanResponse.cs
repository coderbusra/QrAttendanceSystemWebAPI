using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.DTOs.Attendance;

public class AttendanceScanResponse
{
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = null!;
    public DateTime EventDate { get; set; }

    public DateTime ScanTime { get; set; }
    public AttendanceStatus Status { get; set; }

    /// <summary> Kullanıcıya gösterilecek bilgi mesajı </summary>
    public string Message { get; set; } = null!;
}
