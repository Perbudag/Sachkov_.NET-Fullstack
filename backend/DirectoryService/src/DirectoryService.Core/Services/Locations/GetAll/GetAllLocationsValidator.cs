using FluentValidation;

namespace DirectoryService.Core.Services.Locations.GetAll;

public class GetAllLocationsValidator : AbstractValidator<GetAllLocationsQuery>
{
    public GetAllLocationsValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThan(0)
            .WithMessage("Номер страницы должен быть больше нуля")
            .WithErrorCode("locations.validation.error");


        RuleFor(q => q.MinDepartmentCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Минимальное количество подразделений должено быть больше или равным нулю")
            .WithErrorCode("locations.validation.error");

        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Размер страницы должен быть больше нуля и не больше пятидесяти")
            .WithErrorCode("locations.validation.error");
    }
}
