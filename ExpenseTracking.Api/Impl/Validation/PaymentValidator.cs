namespace ExpenseTracking.Api.Impl.Validation;
using FluentValidation;
using ExpenseTracking.Schema;

public class PaymentValidator : AbstractValidator<PaymentRequest>
{
    public PaymentValidator()
    {
        RuleFor(x => x.ExpenseId)
            .NotEmpty().WithMessage("ExpenseId cannot be empty.")
            .GreaterThan(0).WithMessage("ExpenseId must be greater than 0.");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("EmployeeId cannot be empty.")
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount cannot be empty.")
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.IBAN)
            .NotEmpty().WithMessage("IBAN cannot be empty.");
    }
}