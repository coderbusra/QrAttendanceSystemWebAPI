using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        /// <summary>
        /// Kullanıcı rolü: Admin veya User
        /// Frontend burayı DOLDURMAK ZORUNDA.
        /// </summary>
        public UserRole Role { get; set; }
    }
}