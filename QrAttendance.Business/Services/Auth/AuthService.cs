using Microsoft.EntityFrameworkCore;
using QrAttendance.Business.DTOs.Auth;
using QrAttendanceSystem.Business.DTOs.Auth;
using QrAttendanceSystem.Core.Results;
using QrAttendanceSystem.Core.Security;
using QrAttendanceSystem.DataAccess.Context;
using QrAttendanceSystem.Entities;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (existingUser is not null)
        {
            return Result<AuthResponse>.Failure("Email already in use.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.User
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.Name,
            user.Role.ToString()
        );

        var response = new AuthResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),   // enum -> string
            AccessToken = token,
            ExpiresAt = expiresAt
        };

        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure("Invalid credentials.");
        }

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Failure("Invalid credentials.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.Name,
            user.Role.ToString()
        );

        var response = new AuthResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            AccessToken = token,
            ExpiresAt = expiresAt
        };

        return Result<AuthResponse>.Success(response);
    }
}
