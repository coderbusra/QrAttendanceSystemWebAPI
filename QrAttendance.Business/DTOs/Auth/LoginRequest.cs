using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.DTOs.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        /// <summary>
        /// Kullanıcının beklenen rolü.
        /// Frontend login ekranında seçiyor (Admin / User)
        /// </summary>
        public UserRole Role { get; set; }
    }
}