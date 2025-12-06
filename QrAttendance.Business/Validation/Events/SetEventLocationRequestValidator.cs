using FluentValidation;
using QrAttendance.Business.DTOs.Events;
using QrAttendanceSystem.Business.DTOs.Events;
using SetEventLocationRequest = QrAttendanceSystem.Business.DTOs.Events.SetEventLocationRequest;

namespace QrAttendanceSystem.Business.Validation.Events;

public class SetEventLocationRequestValidator : AbstractValidator<SetEventLocationRequest>
{
    public SetEventLocationRequestValidator()
    {
        RuleFor(x => x.Points)
            .NotNull().WithMessage("Points collection is required.")
            .Must(p => p.Count >= 3)
            .WithMessage("At least 3 points are required to define a polygon.");

        RuleForEach(x => x.Points).ChildRules(point =>
        {
            point.RuleFor(p => p.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            point.RuleFor(p => p.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");
        });
    }
}
