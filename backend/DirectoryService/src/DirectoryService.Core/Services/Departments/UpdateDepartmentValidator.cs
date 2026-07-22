using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Presenters;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(d => d.Name!)
            .MustBeValueObject(Name.Create);
    }
}
