using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Positions.Create;

public class CreatePositionValidator : AbstractValidator<CreatePositionRequest>
{
    public CreatePositionValidator()
    {
        RuleFor(p => p.Name)
            .MustBeValueObject(Name.Create);
    }
}
