using FluentValidation;
using System.Text.RegularExpressions;

namespace ExpenseTracking.Api.Validators
{
    public static class RuleBuilderExtensions
    {
        private const string EMAIL_PATTERN =
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$";

        public static IRuleBuilderOptions<T, string> ValidEmail<T>(
            this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Email address is required")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().WithMessage("Invalid email format")
                .Must(BeValidEmail).WithMessage("Email must be in valid format (e.g., user@domain.com)")
                .Must(e => e.Contains("@")).WithMessage("Email must contain '@' symbol")
                .Must(e => e.Split('@')[0].Length >= 3)
                    .WithMessage("Local part must be at least 3 characters")
                .Must(e => !e.StartsWith("."))
                    .WithMessage("Email cannot start with a dot")
                .Must(e => !e.EndsWith("."))
                    .WithMessage("Email cannot end with a dot")
                .Must(e => !e.Contains(".."))
                    .WithMessage("Email cannot contain consecutive dots");
        }

        private static bool BeValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (!Regex.IsMatch(email, EMAIL_PATTERN))
                return false;

            var parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            var (local, domain) = (parts[0], parts[1]);
            if (local.Length > 64 || domain.Length > 255)
                return false;

            if (!domain.Contains("."))
                return false;

            return true;
        }
    }
}
