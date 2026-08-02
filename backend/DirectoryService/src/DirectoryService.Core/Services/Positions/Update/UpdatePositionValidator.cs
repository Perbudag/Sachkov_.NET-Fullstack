using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Positions.Update;

public class UpdatePositionValidator : AbstractValidator<UpdatePositionRequest>
{
    public UpdatePositionValidator()
    {
        RuleFor(p => p.Name!)
            .MustBeValueObject(Name.Create);

    }
}
