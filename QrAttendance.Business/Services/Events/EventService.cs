using Microsoft.EntityFrameworkCore;
using QrAttendanceSystem.Business.DTOs.Events;
using QrAttendanceSystem.Core.Geo;
using QrAttendanceSystem.Core.Results;
using QrAttendanceSystem.Core.Security;
using QrAttendanceSystem.DataAccess.Context;
using QrAttendanceSystem.Entities;

namespace QrAttendanceSystem.Business.Services.Events;

public class EventService : IEventService
{
    private readonly AppDbContext _dbContext;
    private readonly IQrTokenGenerator _qrTokenGenerator;

    public EventService(AppDbContext dbContext, IQrTokenGenerator qrTokenGenerator)
    {
        _dbContext = dbContext;
        _qrTokenGenerator = qrTokenGenerator;
    }

    public async Task<Result<List<EventResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var events = await _dbContext.Events
            .Include(e => e.CreatedByUser)
            .AsNoTracking()
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);

        var response = events.Select(MapToResponse).ToList();
        return Result<List<EventResponse>>.Success(response);
    }

    public async Task<Result<EventResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Events
            .Include(e => e.CreatedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result<EventResponse>.Failure("Event not found.");

        return Result<EventResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<EventResponse>> CreateAsync(
        CreateEventRequest request,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Result<EventResponse>.Failure("Title is required.");

        var qrToken = _qrTokenGenerator.Generate();

        var entity = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Date = request.Date,
            QrToken = qrToken,
            QrExpire = request.QrExpire,
            CreatedByUserId = createdByUserId
        };

        await _dbContext.Events.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // navigation'ı dolduralım
        await _dbContext.Entry(entity)
            .Reference(e => e.CreatedByUser)
            .LoadAsync(cancellationToken);

        return Result<EventResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<EventResponse>> UpdateAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Events
            .Include(e => e.CreatedByUser)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result<EventResponse>.Failure("Event not found.");

        if (string.IsNullOrWhiteSpace(request.Title))
            return Result<EventResponse>.Failure("Title is required.");

        entity.Title = request.Title.Trim();
        entity.Description = request.Description?.Trim();
        entity.Date = request.Date;
        entity.QrExpire = request.QrExpire;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<EventResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure("Event not found.");

        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<EventQrResponse>> GetQrAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result<EventQrResponse>.Failure("Event not found.");

        var response = new EventQrResponse
        {
            EventId = entity.Id,
            QrToken = entity.QrToken,
            QrExpire = entity.QrExpire,
            IsExpired = entity.QrExpire <= DateTime.UtcNow
        };

        return Result<EventQrResponse>.Success(response);
    }

    public async Task<Result> SetLocationAsync(
        Guid id,
        SetEventLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure("Event not found.");

        if (request.Points == null || request.Points.Count < 3)
        {
            // 3'ten az nokta → poligon yok, konum kontrolü yapılmayacak
            entity.LocationPolygonJson = null;
        }
        else
        {
            entity.LocationPolygonJson = PolygonJsonHelper.Serialize(request.Points);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static EventResponse MapToResponse(Event entity)
    {
        var now = DateTime.UtcNow;

        return new EventResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Date = entity.Date,
            QrExpire = entity.QrExpire,
            CreatedAt = entity.CreatedAt,
            CreatedByUserId = entity.CreatedByUserId,
            CreatedByName = entity.CreatedByUser?.Name ?? string.Empty,
            IsQrExpired = entity.QrExpire <= now
        };
    }
}
