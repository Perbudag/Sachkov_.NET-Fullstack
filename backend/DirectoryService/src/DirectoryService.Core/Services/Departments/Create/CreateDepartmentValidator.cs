using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments.Create;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentValidator()
    {

        RuleFor(d => d.Name)
            .MustBeValueObject(Name.Create);

        RuleFor(d => d.Slug)
            .MustBeValueObject(Slug.Create);
    }
}
