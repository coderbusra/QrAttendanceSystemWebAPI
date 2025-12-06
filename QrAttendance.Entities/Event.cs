using QrAttendanceSystem.Entities.Common;

namespace QrAttendanceSystem.Entities;

public class Event : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string QrToken { get; set; } = null!;
    public DateTime QrExpire { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    // Etkinlik alanını temsil eden poligon (JSON formatında)
    public string? LocationPolygonJson { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
