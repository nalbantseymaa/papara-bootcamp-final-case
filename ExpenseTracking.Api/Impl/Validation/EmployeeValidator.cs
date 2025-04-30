using FluentValidation;
using ExpenseTracking.Schema;
using ExpenseTracking.Api.Validators;

namespace ExpenseTracking.Api.Impl.Validation;

public class EmployeeValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Department ID is required.");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.MiddleName).MinimumLength(2)
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
        RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity number is required.").Matches(@"^\d{11}$").WithMessage("Identity number must be 11 digits.");
        RuleFor(x => x.DateOfBirth).NotEmpty().LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");
        RuleFor(x => x.Email).ValidEmail();
    }
}