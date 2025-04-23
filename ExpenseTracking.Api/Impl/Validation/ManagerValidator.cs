using FluentValidation;
using ExpenseTracking.Schema;
using System.Text.RegularExpressions;

namespace ExpenseTracking.Api.Validators;

public class ManagerRequestValidator : AbstractValidator<ManagerRequest>
{
    private const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    public ManagerRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("First name can only contain letters");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("Middle name can only contain letters")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$").WithMessage("Last name can only contain letters");

        RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email address is required")
          .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
          .EmailAddress().WithMessage("Invalid email format")
          .Must(BeValidEmail).WithMessage("Email must be in valid format (e.g., user@domain.com)")
          .Must(email => email.Contains("@")).WithMessage("Email must contain '@' symbol")
          .Must(email => email.Split('@')[0].Length >= 3).WithMessage("Local part must be at least 3 characters")
          .Must(email => !email.StartsWith(".")).WithMessage("Email cannot start with a dot")
          .Must(email => !email.EndsWith(".")).WithMessage("Email cannot end with a dot")
          .Must(email => !email.Contains("..")).WithMessage("Email cannot contain consecutive dots");

    }
    private bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {

            if (!Regex.IsMatch(email, EMAIL_PATTERN))
                return false;

            var parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            var localPart = parts[0];
            var domainPart = parts[1];

            if (localPart.Length > 64)
                return false;

            if (domainPart.Length > 255)
                return false;

            if (!domainPart.Contains("."))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}