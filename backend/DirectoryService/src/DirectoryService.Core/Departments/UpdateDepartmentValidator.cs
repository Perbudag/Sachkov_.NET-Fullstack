using DirectoryService.Domain.ValueObjects;
using DirectoryService.Presenters;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .MinimumLength(Name.MIN_LENGTH)
            .MaximumLength(Name.MAX_LENGTH)
            .WithMessage($"Имя должно содержать от {Name.MIN_LENGTH} до {Name.MAX_LENGTH} символов");
    }
}
