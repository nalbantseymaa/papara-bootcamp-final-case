using ExpenseTracking.Schema;
using FluentValidation;

namespace ExpenseTracking.Api.Impl.Validation;

public class PhoneValidator : AbstractValidator<PhoneRequest>
{
    public PhoneValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .MinimumLength(12)
            .MaximumLength(12)
            .Matches("^[0-9]+$").WithMessage("Phone number must consist of digits only and be exactly 12 characters long.");
        RuleFor(x => x.CountryCode).MinimumLength(3).MaximumLength(3);
    }
}