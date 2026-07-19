using DirectoryService.Contracts.SharedDto;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DirectoryService.Core.Services.Shared.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .WithMessage("Почтовый индекс не должен быть пустым")
            .Matches("^[0-9A-Z -]{4,8}$")
            .WithMessage("Неправильный формат почтового индекса.");

        RuleFor(a => a.Country)
            .NotEmpty()
            .WithMessage("Название страны не должно быть пустым")
            .Matches("^[а-яА-Яa-zA-Z ]+$")
            .WithMessage("Название страны должно состаять только из букв и пробелов");

        RuleFor(a => a.Region)
            .NotEmpty()
            .WithMessage("Название региона не должно быть пустым")
            .Matches("^[а-яА-Яa-zA-Z ]+$")
            .WithMessage("Название региона должно состаять только из букв и пробелов");

        RuleFor(a => a.City)
            .NotEmpty()
            .WithMessage("Название города не должно быть пустым")
            .Matches("^[а-яА-Яa-zA-Z ]+$")
            .WithMessage("Название города должно состаять только из букв и пробелов");

        RuleFor(a => a.Street)
            .NotEmpty()
            .WithMessage("Название улицы не должно быть пустым")
            .Matches("^[а-яА-Яa-zA-Z ]+$")
            .WithMessage("Название улицы должно состаять только из букв и пробелов");

        RuleFor(a => a.House)
            .NotEmpty()
            .WithMessage("Номер дома не должен быть пустым")
            .Matches("^[0-9а-яa-z]+$")
            .WithMessage("Номер дома должен состаять только из прописных букв и/или цифр");

        RuleFor(a => a.Apartment)
            .Matches("^[0-9а-яa-z]+$")
            .WithMessage("Номер квартиры должен состаять только из прописных букв и/или цифр");
    }
}
