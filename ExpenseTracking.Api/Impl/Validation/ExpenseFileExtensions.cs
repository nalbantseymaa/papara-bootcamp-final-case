using FluentValidation;

namespace ExpenseTracking.Api.Validators
{
    public static class ExpenseFileExtensions
    {
        private const int MaxFileSizeInMB = 10;

        public static IRuleBuilderOptions<T, IFormFile> ValidFile<T>(
            this IRuleBuilder<T, IFormFile> rule)
        {
            return rule
                .NotNull().WithMessage("File selection is required")
                .Must(file => file.Length > 0).WithMessage("File cannot be empty")
                .Must(BeValidFileSize).WithMessage($"File size cannot exceed {MaxFileSizeInMB}MB")
                .Must(BeValidFileType).WithMessage("Only PDF, JPG, and PNG file types are accepted");
        }

        private static bool BeValidFileSize(IFormFile file)
        {
            return file.Length <= MaxFileSizeInMB * 1024 * 1024;
        }

        private static bool BeValidFileType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => true,
                ".jpg" or ".jpeg" => true,
                ".png" => true,
                _ => false
            };
        }
    }
}