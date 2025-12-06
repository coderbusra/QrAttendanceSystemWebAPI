using System.Text.Json;

namespace QrAttendanceSystem.Core.Geo;

public static class PolygonJsonHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize(IReadOnlyList<GeoPoint> points)
    {
        return JsonSerializer.Serialize(points, Options);
    }

    public static List<GeoPoint> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<GeoPoint>();

        try
        {
            var points = JsonSerializer.Deserialize<List<GeoPoint>>(json, Options);
            return points ?? new List<GeoPoint>();
        }
        catch
        {
            // JSON bozuksa boş liste dönüyoruz (konum kontrolü yapılmaz)
            return new List<GeoPoint>();
        }
    }
}
