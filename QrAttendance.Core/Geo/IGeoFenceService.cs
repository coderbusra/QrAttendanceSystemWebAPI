namespace QrAttendanceSystem.Core.Geo;

public interface IGeoFenceService
{
    bool IsInside(GeoPoint point, IReadOnlyList<GeoPoint> polygon);
}
