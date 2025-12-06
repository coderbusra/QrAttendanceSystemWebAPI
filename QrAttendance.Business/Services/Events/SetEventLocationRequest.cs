using QrAttendanceSystem.Core.Geo;

namespace QrAttendanceSystem.Business.DTOs.Events;

public class SetEventLocationRequest
{
    // En az 3 nokta → polygon
    public List<GeoPoint> Points { get; set; } = new();
}
