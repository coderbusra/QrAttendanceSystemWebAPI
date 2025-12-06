namespace QrAttendanceSystem.Core.Options;

public class AdminUserSettings
{
    public const string SectionName = "AdminUser";

    public string Name { get; set; } = "System Admin";
    public string Email { get; set; } = "admin@qrattendance.local";
    public string Password { get; set; } = "Admin123!";
}
