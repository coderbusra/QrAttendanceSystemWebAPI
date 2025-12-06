using QrAttendanceSystem.Core.Entities;

namespace QrAttendanceSystem.Entities.Common;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
