using FluentValidation;
using ExpenseTracking.Schema;
using ExpenseTracking.Api.Validators;

namespace ExpenseTracking.Api.Impl.Validation;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("DepartmentId must be greater than 0");
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than 0");
    }
}