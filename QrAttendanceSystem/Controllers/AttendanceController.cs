using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrAttendanceSystem.Business.DTOs.Attendance;
using QrAttendanceSystem.Business.Services.Attendance;

namespace QrAttendanceSystem.Api.Controllers;

[ApiController]
[Route("attendance")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // POST /attendance/scan
    [HttpPost("scan")]
    public async Task<IActionResult> Scan([FromBody] AttendanceScanRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var result = await _attendanceService.ScanAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    // GET /attendance/event/{id}
    [Authorize(Roles = "Admin")]
    [HttpGet("event/{id:guid}")]
    public async Task<IActionResult> GetByEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetByEventAsync(id, cancellationToken);
        return Ok(result.Value);
    }

    // GET /attendance/user/me
    [HttpGet("user/me")]
    public async Task<IActionResult> GetMyAttendances(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _attendanceService.GetByUserAsync(userId, cancellationToken);
        return Ok(result.Value);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidOperationException("User id claim is missing.");

        return Guid.Parse(userId);
    }
}
