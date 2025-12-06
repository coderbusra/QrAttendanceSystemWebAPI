using QrAttendanceSystem.Core.Geo;

namespace QrAttendanceSystem.Business.Geo;

public class RayCastingGeoFenceService : IGeoFenceService
{
    public bool IsInside(GeoPoint point, IReadOnlyList<GeoPoint> polygon)
    {
        if (polygon is null || polygon.Count < 3)
            return true; // poligon yoksa konum kontrolü yapma => kabul et

        var inside = false;

        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            var xi = polygon[i].Longitude;
            var yi = polygon[i].Latitude;
            var xj = polygon[j].Longitude;
            var yj = polygon[j].Latitude;

            var intersect = ((yi > point.Latitude) != (yj > point.Latitude)) &&
                            (point.Longitude < (xj - xi) * (point.Latitude - yi) / (yj - yi + double.Epsilon) + xi);

            if (intersect)
                inside = !inside;
        }

        return inside;
    }
}
