using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Shared;

public class Failure : Collection<Error>
{

    [JsonConstructor]
    public Failure(IEnumerable<Error> errors) : base(errors.ToList())
    {
    }

    public Failure()
    {
    }

    public static implicit operator Failure(Error error) =>
        new([error]);
}
