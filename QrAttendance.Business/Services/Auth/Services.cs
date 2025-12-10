using QrAttendanceSystem.Business.DTOs.Auth;
using QrAttendanceSystem.Core.Results;

namespace QrAttendanceSystem.Business.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
