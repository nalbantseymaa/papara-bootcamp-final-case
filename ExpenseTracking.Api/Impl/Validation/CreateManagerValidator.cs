using ExpenseTracking.Schema;
using FluentValidation;

public class CreateManagerRequestValidator
    : AbstractValidator<CreateManagerRequest>
{
    public CreateManagerRequestValidator(
        IValidator<UserRequest> userValidator,
        IValidator<ManagerRequest> mngValidator)
    {
        RuleFor(x => x.User).NotNull().SetValidator(userValidator);
        RuleFor(x => x.Manager).NotNull().SetValidator(mngValidator);
    }
}
