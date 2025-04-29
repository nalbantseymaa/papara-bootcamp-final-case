using System.Data;
using ExpenseTracking.Schema;
using FluentValidation;

namespace ExpenseTracking.Api.Impl.Validation;

public class AddressValidator : AbstractValidator<AddressRequest>
{
    public AddressValidator()
    {
        RuleFor(x => x.IsDefault).NotNull().WithMessage("Please enter a valid default address status");
        RuleFor(x => x.Street).NotEmpty().Length(2, 50).WithMessage("Please enter a valid street name");
        RuleFor(x => x.District).NotEmpty().Length(2, 50).WithMessage("Please enter a valid district name");
        RuleFor(x => x.City).NotEmpty().Length(2, 50).WithMessage("Please enter a valid city name");
        RuleFor(x => x.CountryCode).NotEmpty().Length(3).WithMessage("Please enter a valid country code");
        RuleFor(x => x.ZipCode).NotEmpty().Matches("[0-9]+").WithMessage("Please enter a valid zip code");
    }
}
