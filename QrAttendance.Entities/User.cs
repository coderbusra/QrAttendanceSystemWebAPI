using QrAttendanceSystem.Entities.Common;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }

    // Navigation Properties
    public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
