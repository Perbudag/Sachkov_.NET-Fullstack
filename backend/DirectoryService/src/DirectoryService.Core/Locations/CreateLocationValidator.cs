using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Shared.Validators;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations
{
    internal class CreateLocationValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationValidator()
        {
            RuleFor(l => l.Name.Trim())
                .MinimumLength(Name.MIN_LENGTH)
                .Length(Name.MAX_LENGTH)
                .WithMessage($"Имя должно содержать от {Name.MIN_LENGTH} до {Name.MAX_LENGTH} символов");

            RuleFor(l => l.Address).SetValidator(new AddressDtoValidator());
        }
    }
}
