using QrAttendanceSystem.Business.DTOs.Attendance;
using QrAttendanceSystem.Core.Results;

namespace QrAttendanceSystem.Business.Services.Attendance;

public interface IAttendanceService
{
    Task<Result<AttendanceScanResponse>> ScanAsync(
        Guid userId,
        AttendanceScanRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<List<AttendanceRecordResponse>>> GetByEventAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<Result<List<AttendanceRecordResponse>>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
