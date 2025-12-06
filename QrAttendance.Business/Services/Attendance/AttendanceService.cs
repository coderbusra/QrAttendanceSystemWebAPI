using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QrAttendanceSystem.Business.DTOs.Attendance;
using QrAttendanceSystem.Business.Services.Attendance;
using QrAttendanceSystem.Core.Geo;
using QrAttendanceSystem.Core.Results;
using QrAttendanceSystem.DataAccess.Context;
using QrAttendanceSystem.Entities;
using QrAttendanceSystem.Entities.Enums;

namespace QrAttendanceSystem.Business.Services.Attendances;

public class AttendanceService : IAttendanceService
{
    private readonly AppDbContext _dbContext;
    private readonly IGeoFenceService _geoFenceService;

    public AttendanceService(AppDbContext dbContext, IGeoFenceService geoFenceService)
    {
        _dbContext = dbContext;
        _geoFenceService = geoFenceService;
    }

    public async Task<Result<AttendanceScanResponse>> ScanAsync(
        Guid userId,
        AttendanceScanRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.QrToken))
            return Result<AttendanceScanResponse>.Failure("QR token is required.");

        // QR token üzerinden event bul
        var @event = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.QrToken == request.QrToken, cancellationToken);

        if (@event is null)
            return Result<AttendanceScanResponse>.Failure("Invalid QR token.");

        var now = DateTime.UtcNow;
        var status = AttendanceStatus.Success;
        var message = "Attendance recorded successfully.";

        // 1) QR süresi kontrolü
        if (@event.QrExpire <= now)
        {
            status = AttendanceStatus.QrExpired;
            message = "QR code has expired.";
        }
        else
        {
            // 2) Poligon konum kontrolü
            if (!string.IsNullOrWhiteSpace(@event.LocationPolygonJson))
            {
                var point = new GeoPoint(request.Latitude, request.Longitude);

                var polygon = DeserializePolygon(@event.LocationPolygonJson);
                var isInside = _geoFenceService.IsInside(point, polygon);

                if (!isInside)
                {
                    status = AttendanceStatus.OutOfLocation;
                    message = "You are not in the event area.";
                }
            }

            // 3) Daha önce başarılı bir yoklama var mı?
            if (status == AttendanceStatus.Success)
            {
                var alreadyExists = await _dbContext.Attendances
                    .AnyAsync(a =>
                        a.EventId == @event.Id &&
                        a.UserId == userId &&
                        a.Status == AttendanceStatus.Success,
                        cancellationToken);

                if (alreadyExists)
                {
                    status = AttendanceStatus.AlreadyScanned;
                    message = "Attendance has already been recorded for this event.";
                }
            }
        }

        var attendance = new Entities.Attendance
        {
            EventId = @event.Id,
            UserId = userId,
            ScanTime = now,
            Status = status,
            DeviceInfo = request.DeviceInfo
        };

        await _dbContext.Attendances.AddAsync(attendance, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AttendanceScanResponse
        {
            EventId = @event.Id,
            EventTitle = @event.Title,
            EventDate = @event.Date,
            ScanTime = attendance.ScanTime,
            Status = status,
            Message = message
        };

        return Result<AttendanceScanResponse>.Success(response);
    }

    public async Task<Result<List<AttendanceRecordResponse>>> GetByEventAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Attendances
            .AsNoTracking()
            .Include(a => a.Event)
            .Include(a => a.User)
            .Where(a => a.EventId == eventId)
            .OrderByDescending(a => a.ScanTime);

        var list = await query.ToListAsync(cancellationToken);

        var response = list.Select(a => new AttendanceRecordResponse
        {
            AttendanceId = a.Id,
            EventId = a.EventId,
            EventTitle = a.Event.Title,
            EventDate = a.Event.Date,
            UserId = a.UserId,
            UserName = a.User.Name,
            UserEmail = a.User.Email,
            ScanTime = a.ScanTime,
            Status = a.Status,
            DeviceInfo = a.DeviceInfo
        }).ToList();

        return Result<List<AttendanceRecordResponse>>.Success(response);
    }

    public async Task<Result<List<AttendanceRecordResponse>>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Attendances
            .AsNoTracking()
            .Include(a => a.Event)
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ScanTime);

        var list = await query.ToListAsync(cancellationToken);

        var response = list.Select(a => new AttendanceRecordResponse
        {
            AttendanceId = a.Id,
            EventId = a.EventId,
            EventTitle = a.Event.Title,
            EventDate = a.Event.Date,
            UserId = a.UserId,
            UserName = a.User.Name,
            UserEmail = a.User.Email,
            ScanTime = a.ScanTime,
            Status = a.Status,
            DeviceInfo = a.DeviceInfo
        }).ToList();

        return Result<List<AttendanceRecordResponse>>.Success(response);
    }

    private static List<GeoPoint> DeserializePolygon(string json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var points = JsonSerializer.Deserialize<List<GeoPoint>>(json, options);
            return points ?? new List<GeoPoint>();
        }
        catch
        {
            // JSON bozuksa konum kontrolü yapmayalım
            return new List<GeoPoint>();
        }
    }
}
