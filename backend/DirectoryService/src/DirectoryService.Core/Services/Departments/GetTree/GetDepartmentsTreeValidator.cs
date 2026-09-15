using DirectoryService.Core.Services.Departments.GetDepartmentsTree;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments.GetTree;

public class GetDepartmentsTreeValidator : AbstractValidator<GetDepartmentsTreeQuery>
{
    public GetDepartmentsTreeValidator()
    {
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
