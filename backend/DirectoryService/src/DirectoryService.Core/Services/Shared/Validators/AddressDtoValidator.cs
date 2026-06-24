using DirectoryService.Contracts.SharedDto;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DirectoryService.Core.Services.Shared.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    private readonly Regex _postalCodePattern = new Regex("^[0-9A-Z -]{4,8}$");
    private readonly Regex _namePattern = new Regex("^[а-яА-Яa-zA-Z ]+$");
    private readonly Regex _numberPattern = new Regex("^[0-9а-яa-z]+$");

    public AddressDtoValidator()
    {
        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .WithMessage("Почтовый индекс не должен быть пустым")
            .Matches(_postalCodePattern)
            .WithMessage("Неправильный формат почтового индекса.");

        RuleFor(a => a.Country)
            .NotEmpty()
            .WithMessage("Название страны не должно быть пустым")
            .Matches(_namePattern)
            .WithMessage("Название страны должно состаять только из букв и пробелов");

        RuleFor(a => a.Region)
            .NotEmpty()
            .WithMessage("Название региона не должно быть пустым")
            .Matches(_namePattern)
            .WithMessage("Название региона должно состаять только из букв и пробелов");

        RuleFor(a => a.City)
            .NotEmpty()
            .WithMessage("Название города не должно быть пустым")
            .Matches(_namePattern)
            .WithMessage("Название города должно состаять только из букв и пробелов");

        RuleFor(a => a.Street)
            .NotEmpty()
            .WithMessage("Название улицы не должно быть пустым")
            .Matches(_namePattern)
            .WithMessage("Название улицы должно состаять только из букв и пробелов");

        RuleFor(a => a.House)
            .NotEmpty()
            .WithMessage("Номер дома не должен быть пустым")
            .Matches(_numberPattern)
            .WithMessage("Номер дома должен состаять только из прописных букв и/или цифр");

        RuleFor(a => a.Apartment)
            .Matches(_numberPattern)
            .WithMessage("Номер квартиры должен состаять только из прописных букв и/или цифр");
    }
}
