using QrAttendanceSystem.Business.DTOs.Events;
using QrAttendanceSystem.Core.Results;

namespace QrAttendanceSystem.Business.Services.Events;

public interface IEventService
{
    Task<Result<List<EventResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<EventResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<EventResponse>> CreateAsync(
        CreateEventRequest request,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    Task<Result<EventResponse>> UpdateAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<EventQrResponse>> GetQrAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result> SetLocationAsync(
        Guid id,
        SetEventLocationRequest request,
        CancellationToken cancellationToken = default);
}
