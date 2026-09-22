using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DirectoryService.Core.Services.Departments.GetAncestors;

public class GetDepartmentAncestorsValidator : AbstractValidator<GetDepartmentAncestorsQuery>
{
    private readonly List<string> _allowedValuesSortDir = ["asc", "desc"];

    public GetDepartmentAncestorsValidator()
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

        RuleFor(q => q.SortDir)
            .Must(sd => _allowedValuesSortDir.Contains(sd, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Значениями SortDir могут быть только \"asc\" и \"desc\"")
            .WithErrorCode("departments.validation.error");
    }
}
