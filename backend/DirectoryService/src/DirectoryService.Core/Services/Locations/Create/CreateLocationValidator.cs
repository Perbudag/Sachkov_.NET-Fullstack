using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Locations.Create;

public class CreateLocationValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .MustBeValueObject(Name.Create);

        RuleFor(l => l.Address)
            .MustBeValueObject(address => Address.Create(
                address.PostalCode,
                address.Country,
                address.Region,
                address.City,
                address.Street,
                address.House,
                address.Apartment));
    }
}
