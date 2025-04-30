using ExpenseTracking.Schema;
using FluentValidation;

public class CreateEmployeeRequestValidator
    : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(
        IValidator<UserRequest> userValidator,
        IValidator<EmployeeRequest> empValidator)
    {
        RuleFor(x => x.User).NotNull().SetValidator(userValidator);
        RuleFor(x => x.Employee).NotNull().SetValidator(empValidator);
    }
}
