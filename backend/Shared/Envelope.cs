using System.Text.Json.Serialization;

namespace Shared;

public record Envelope<T>
{
    public T? Result { get; }

    public Failure? Errors { get; }

    public DateTime TimeGenerated { get; }

    [JsonIgnore]
    public bool IsError => Errors != null && Errors.Any();

    [JsonConstructor]
    internal Envelope(T? result, Failure? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }
}

public record Envelope : Envelope<object>
{
    [JsonConstructor]
    internal Envelope(object? result, Failure? errors) : base(result, errors)
    {
    }

    public static Envelope Ok(object result) =>
        new(result, null);

    public static Envelope<T> Ok<T>(T result) =>
        new Envelope<T>(result, null);


    public static Envelope Failure(Failure errors) =>
        new(null, errors);

    public static Envelope<T> Failure<T>(Failure errors) =>
        new Envelope<T>(default, errors);
}