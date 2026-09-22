using DirectoryService.Core.Services.Departments.GetDepartmentsTree;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments.GetChildren;

public class GetDepartmentChildrenValidator : AbstractValidator<GetDepartmentChildrenQuery>
{
    public GetDepartmentChildrenValidator()
    {
        RuleFor(q => q.ParentId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id родителя не должен быть нулевым")
            .WithErrorCode("departments.validation.error");

        RuleFor(q => q.Page)
            .GreaterThan(0)
            .WithMessage("Номер страницы должен быть больше нуля")
            .WithErrorCode("departments.validation.error");

        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Размер страницы должен быть больше нуля и не больше пятидесяти")
            .WithErrorCode("departments.validation.error");
    }
}
