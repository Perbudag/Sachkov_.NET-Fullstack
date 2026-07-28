using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments.Update;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(d => d.Name!)
            .MustBeValueObject(Name.Create);
    }
}
