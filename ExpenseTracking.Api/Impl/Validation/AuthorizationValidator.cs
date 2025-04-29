using ExpenseTracking.Api.Schema;
using FluentValidation;

namespace ExpenseTracking.Api.Impl.Validation;

public class AuthorizationValidator : AbstractValidator<AuthorizationRequest>
{
    public AuthorizationValidator()
    {
        RuleFor(x => x.UserName).MinimumLength(5).MaximumLength(50);
        RuleFor(x => x.Password).MinimumLength(5).MaximumLength(50);
    }
}