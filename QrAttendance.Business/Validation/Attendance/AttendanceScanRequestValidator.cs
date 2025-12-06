using FluentValidation;
using QrAttendanceSystem.Business.DTOs.Attendance;

namespace QrAttendanceSystem.Business.Validation.Attendance;

public class AttendanceScanRequestValidator : AbstractValidator<AttendanceScanRequest>
{
    public AttendanceScanRequestValidator()
    {
        RuleFor(x => x.QrToken)
            .NotEmpty().WithMessage("QrToken is required.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.DeviceInfo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceInfo));
    }
}
