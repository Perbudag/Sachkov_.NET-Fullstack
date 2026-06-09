using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentValidator()
    {

        RuleFor(d => d.Name)
            .NotNull()
            .MinimumLength(Name.MIN_LENGTH)
            .MaximumLength(Name.MAX_LENGTH)
            .WithMessage($"Имя должно содержать от {Name.MIN_LENGTH} до {Name.MAX_LENGTH} символов");

        RuleFor(d => d.Slug)
            .NotNull()
            .MinimumLength(Slug.MIN_LENGTH)
            .MaximumLength(Slug.MAX_LENGTH)
            .WithMessage($"Slug должен содержать от {Name.MIN_LENGTH} до {Name.MAX_LENGTH} символов")
            .Matches("^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")
            .WithMessage($"Slug может состоять из латинских букв, цифр и дефисов, " +
                          "а также, не начинается и не заканчивается дефисом");
    }
}
