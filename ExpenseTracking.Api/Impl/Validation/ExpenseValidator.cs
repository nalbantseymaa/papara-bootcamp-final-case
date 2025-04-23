using FluentValidation;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Validators;

public class ExpenseRequestValidator : AbstractValidator<ExpenseRequest>
{
    public ExpenseRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required")
            .GreaterThan(0).WithMessage("Employee ID must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required")
            .GreaterThan(0).WithMessage("Category ID must be greater than 0");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("Payment method ID is required")
            .GreaterThan(0).WithMessage("Payment method ID must be greater than 0");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThan(1000000).WithMessage("Amount cannot exceed 1,000,000");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(250).WithMessage("Location cannot exceed 250 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ExpenseDate)
            .NotEmpty().WithMessage("Expense date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Expense date cannot be in the future");
    }
}