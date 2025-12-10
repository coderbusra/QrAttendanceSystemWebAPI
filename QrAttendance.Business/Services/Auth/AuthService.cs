using Microsoft.EntityFrameworkCore;
using QrAttendanceSystem.Business.DTOs.Auth;
using QrAttendanceSystem.Core.Results;
using QrAttendanceSystem.Core.Security;
using QrAttendanceSystem.DataAccess.Context;
using QrAttendanceSystem.Entities;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.Services.Auth
{
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

        // REGISTER
        public async Task<Result<AuthResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (exists)
                return Result<AuthResponse>.Failure("Email is already in use.");

            // İSTEDİĞİN MANTIK BURADA:
            // Frontend'in gönderdiği role'ü kullanıcıya yazıyoruz.
            // Güvenlik istersen, burada "Admin sadece Admin endpointinden oluşturulabilir" gibi
            // ekstra kurallar ekleyebilirsin.
            var roleToAssign = request.Role;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                Role = roleToAssign
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Email,
                user.Name,
                user.Role.ToString()
            );

            var response = new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return Result<AuthResponse>.Success(response);
        }

        // LOGIN
        public async Task<Result<AuthResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user is null)
                return Result<AuthResponse>.Failure("Invalid email or password.");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure("Invalid email or password.");

            // 🔐 ÖNEMLİ: Frontend'in gönderdiği role ile DB'deki rolü karşılaştırıyoruz.
            // Böylece aynı email/password için yanlış rol seçerse login başarısız olur.
            if (user.Role != request.Role)
            {
                return Result<AuthResponse>.Failure("Role mismatch for this user.");
            }

            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Email,
                user.Name,
                user.Role.ToString()
            );

            var response = new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return Result<AuthResponse>.Success(response);
        }
    }
}