using QrAttendanceSystem.Entities.Common;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Entities;

public class Attendance : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ScanTime { get; set; } = DateTime.UtcNow;
    public AttendanceStatus Status { get; set; }

    // Cihaz hakkında basic bilgi (model, platform vs.)
    public string? DeviceInfo { get; set; }
}
