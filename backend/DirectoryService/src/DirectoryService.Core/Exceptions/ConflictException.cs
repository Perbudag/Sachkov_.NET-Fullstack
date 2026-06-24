using Shared;
using System.Text.Json;

namespace DirectoryService.Core.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(IEnumerable<Error> errors) : base(JsonSerializer.Serialize(errors))
    {

    }
}
