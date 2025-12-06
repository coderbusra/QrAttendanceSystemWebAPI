using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.DTOs.Attendance;

public class AttendanceRecordResponse
{
    public Guid AttendanceId { get; set; }

    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = null!;
    public DateTime EventDate { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;

    public DateTime ScanTime { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? DeviceInfo { get; set; }
}
