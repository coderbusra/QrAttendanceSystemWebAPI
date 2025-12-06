namespace QrAttendanceSystem.Entities.Enums;

public enum AttendanceStatus
{
    Success = 1,        // Geçerli okuma
    AlreadyScanned = 2, // Aynı kullanıcı aynı etkinlik için tekrar okuttu
    QrExpired = 3,      // QR süresi geçmiş
    OutOfLocation = 4,  // Tanımlı poligon dışında okuma
    InvalidQr = 5       // Token geçersiz vb.
}
