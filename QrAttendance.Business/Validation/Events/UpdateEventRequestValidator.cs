using FluentValidation;
using QrAttendanceSystem.Business.DTOs.Events;

namespace QrAttendanceSystem.Business.Validation.Events;

public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
{
    public UpdateEventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.QrExpire)
            .NotEmpty().WithMessage("QrExpire is required.")
            .Must(d => d > DateTime.UtcNow)
            .WithMessage("QrExpire must be in the future.");
    }
}
