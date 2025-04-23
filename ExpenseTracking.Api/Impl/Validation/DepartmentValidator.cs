using FluentValidation;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Validators;

public class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MinimumLength(2).WithMessage("Department name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s]*$").WithMessage("Department name can only contain letters, numbers and spaces");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ManagerId)
            .NotEmpty().WithMessage("Manager ID is required")
            .GreaterThan(0).WithMessage("Manager ID must be greater than 0");
    }
}