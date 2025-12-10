using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QrAttendanceSystem.Api.Extensions;
using QrAttendanceSystem.Api.Middleware;
using QrAttendanceSystem.Business;
using QrAttendanceSystem.Business.Geo;
using QrAttendanceSystem.Business.Security;
using QrAttendanceSystem.Business.Services.Attendance;
using QrAttendanceSystem.Business.Services.Attendances;
using QrAttendanceSystem.Business.Services.Auth;
using QrAttendanceSystem.Business.Services.Events;
using QrAttendanceSystem.Business.Validation.Auth;
using QrAttendanceSystem.Core.Geo;
using QrAttendanceSystem.Core.Options;
using QrAttendanceSystem.Core.Security;
using QrAttendanceSystem.DataAccess.Context;

var builder = WebApplication.CreateBuilder(args);

// Jwt Settings binding
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// Admin User Settings binding
builder.Services.Configure<AdminUserSettings>(
    builder.Configuration.GetSection(AdminUserSettings.SectionName));

// DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("QrAttendance.DataAccess"));
});

// Business services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IQrTokenGenerator, QrTokenGenerator>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IGeoFenceService, RayCastingGeoFenceService>();

// ---------------------------------------------------------
// 1. ADIM: CORS SERVİSİNİ EKLE (Controller'lardan önce)
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin());
});
// ---------------------------------------------------------

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred."
        };

        return new BadRequestObjectResult(problemDetails);
    };
});

// JWT Authentication
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>() ?? new JwtSettings();

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

// Claim mapping’i temizle
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev ortamı
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddControllers();

// Swagger + JWT security
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QrAttendanceSystem API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// ---------------------------------------------------------
// 2. ADIM: CORS MIDDLEWARE'İ KULLAN (Auth'dan ÖNCE olmalı)
// ---------------------------------------------------------
app.UseCors("AllowAll");
// ---------------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();

// Migration + Admin seeding
await app.ApplyMigrationsAndSeedAsync();

app.MapControllers();

app.Run();