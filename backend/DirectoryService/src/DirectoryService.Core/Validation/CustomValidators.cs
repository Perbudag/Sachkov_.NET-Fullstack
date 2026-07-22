using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Shared;
using System.Data;
using System.Text.Json;

namespace DirectoryService.Core.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Failure>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (value is null)
                return;

            Result<TValueObject, Failure> result = factoryMethod.Invoke(value);

            if (result.IsSuccess)
                return;

            foreach (var item in result.Error)
            {
                context.AddFailure(new ValidationFailure
                {
                    CustomState = item.Type,
                    ErrorCode = item.Code,
                    ErrorMessage = item.Message,
                    PropertyName = item.InvalidField ?? context.DisplayName
                });
            }
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder, Error error)
    {
        ruleBuilder.WithErrorCode(error.Code);
        ruleBuilder.WithMessage(error.Message);

        return ruleBuilder;
    }
}
