using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QrAttendanceSystem.DataAccess.Context;

namespace QrAttendanceSystem.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
