namespace QrAttendanceSystem.Business.DTOs.Attendance;

public class AttendanceScanRequest
{
    public string QrToken { get; set; } = null!;

    /// <summary> Kullanıcının o anki enlem değeri </summary>
    public double Latitude { get; set; }

    /// <summary> Kullanıcının o anki boylam değeri </summary>
    public double Longitude { get; set; }

    /// <summary> Cihaz bilgisi: "Android - Pixel 7", "iOS - iPhone 14" vb. </summary>
    public string? DeviceInfo { get; set; }
}
