using FluentValidation;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Validators;

public class ExpenseFileValidator : AbstractValidator<ExpenseFileRequest>
{
    private const int MaxFileSizeInMB = 10;

    public ExpenseFileValidator()
    {
        RuleFor(x => x.ExpenseId)
            .NotEmpty().WithMessage("Expense ID is required")
            .GreaterThan(0).WithMessage("A valid Expense ID must be provided");

        RuleFor(x => x.File).ValidFile();
    }
}

public class UpdateExpenseFileValidator : AbstractValidator<UpdateExpenseFileRequest>
{
    private const int MaxFileSizeInMB = 10;

    public UpdateExpenseFileValidator()
    {
        RuleFor(x => x.File).ValidFile();
    }
}