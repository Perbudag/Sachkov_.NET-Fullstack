using DirectoryService.Core.Services.Departments.SearchDepartments;
using FluentValidation;

namespace DirectoryService.Core.Services.Departments.SearchTreeDepartments;

public class SearchTreeDepartmentsValidator : AbstractValidator<SearchTreeDepartmentsQuery>
{
    public SearchTreeDepartmentsValidator()
    {
        RuleFor(q => q.Q)
            .MinimumLength(2)
            .WithMessage("Длина запроса должна быть больше 2 символов")
            .WithErrorCode("departments.validation.error");
    }
}
