using Shared.Kernel;

namespace DirectoryService.Domain.Errors;

public static class SharedErrors
{
    // Обязательность поля (уже был)
    public static Error IsRequired(string invalidField, string? code = null) =>
        Error.Validation($"{invalidField} is required.", code, invalidField);

    // Универсальный шаблон для проверки диапазонов длины (для Name, Slug и др.)
    public static Error InvalidLength(string invalidField, int min, int max, string? code = null) =>
        Error.Validation($"{invalidField} length must be between {min} and {max} characters.", code, invalidField);

    // Универсальный шаблон для нарушения формата / регулярных выражений
    public static Error InvalidFormat(string invalidField, string requirements, string? code = null) =>
        Error.Validation($"{invalidField} format is invalid. Requirements: {requirements}", code, invalidField);

    // Универсальный шаблон для некорректной структуры строки при парсинге
    public static Error InvalidStringStructure(string invalidField, string correctExample, string? code = null) =>
        Error.Validation($"Invalid structure for {invalidField}. Correct format example: \"{correctExample}\"", code);
}