using FluentValidation;

namespace DirectoryService.Core.Services.Departments.ChangeParent;

public class ChangeDepartmentParentValidator : AbstractValidator<ChangeDepartmentParentCommand>
{
    public ChangeDepartmentParentValidator()
    {
        RuleFor(d => d.Id)
            .Must((command, id) => id != command.Request.ParentId)
            .WithMessage("Отделение не может быть родителем для самого себя")
            .WithErrorCode("department.move.parent_is_self");
    }
}
