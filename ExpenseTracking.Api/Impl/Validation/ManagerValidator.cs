using FluentValidation;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Validators;

public class ManagerRequestValidator : AbstractValidator<ManagerRequest>
{
    public ManagerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("First name can only contain letters");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("Middle name can only contain letters")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("Last name can only contain letters");


    }
}