using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QrAttendanceSystem.Core.Options;
using QrAttendanceSystem.Core.Security;
using QrAttendanceSystem.DataAccess.Context;
using QrAttendanceSystem.Entities;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();

            // 1) Migration’ları otomatik uygula (dev’de güzel, prod’da istersen kapat)
            await dbContext.Database.MigrateAsync();

            // 2) Admin seeding
            var adminSettings = services
                .GetRequiredService<IOptions<AdminUserSettings>>()
                .Value;

            var passwordHasher = services.GetRequiredService<IPasswordHasher>();

            var adminEmail = adminSettings.Email.Trim().ToLowerInvariant();

            var existingAdmin = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (existingAdmin is null)
            {
                var adminUser = new User
                {
                    Name = adminSettings.Name.Trim(),
                    Email = adminEmail,
                    PasswordHash = passwordHasher.Hash(adminSettings.Password),
                    Role = UserRole.Admin
                };

                await dbContext.Users.AddAsync(adminUser);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Admin user created with email: {Email}", adminUser.Email);
            }
            else
            {
                logger.LogInformation("Admin user already exists with email: {Email}", existingAdmin.Email);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations or seeding the database.");
            throw; // Uygulama ayağa kalkmasın, hatayı bilerek fırlatıyoruz
        }
    }
}
