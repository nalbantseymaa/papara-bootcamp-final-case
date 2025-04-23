namespace ExpenseTracking.Api.Impl.Validation;
using FluentValidation;
using ExpenseTracking.Schema;

public class PaymentMethodValidator : AbstractValidator<PaymentMethodRequest>
{
    public PaymentMethodValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name field cannot be empty.")
           .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
           .MaximumLength(50).WithMessage("Name must be at most 50 characters long.");

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description must be at most 500 characters long.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must be at least 10 characters long if provided.");

    }
}