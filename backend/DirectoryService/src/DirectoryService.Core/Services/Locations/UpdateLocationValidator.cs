using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Services.Shared.Validators;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Locations;

public class UpdateLocationValidator : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationValidator()
    {
        RuleFor(l => l.Name)
            .MinimumLength(Name.MIN_LENGTH)
            .MaximumLength(Name.MAX_LENGTH)
            .WithMessage($"Имя должно содержать от {Name.MIN_LENGTH} до {Name.MAX_LENGTH} символов");

        RuleFor(l => l.Address!).SetValidator(new AddressDtoValidator());
    }
}
