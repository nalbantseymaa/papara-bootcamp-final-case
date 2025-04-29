namespace ExpenseTracking.Api.Impl.Validation;
using FluentValidation;
using ExpenseTracking.Schema;

public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required")
            .MinimumLength(3).WithMessage("UserName must be at least 3 characters long")
            .MaximumLength(50).WithMessage("UserName must not exceed 100 characters");
    }
}