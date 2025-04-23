using FluentValidation;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Validators;

public class RejectExpenseRequestValidator : AbstractValidator<RejectExpenseRequest>
{
    public RejectExpenseRequestValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required")
            .MinimumLength(10).WithMessage("Rejection reason must be at least 10 characters")
            .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters")
            .Must(ContainValidExplanation)
            .WithMessage("Rejection reason must contain a valid explanation");
    }

    private bool ContainValidExplanation(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return false;

        var words = reason.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 2;
    }
}