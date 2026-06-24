using Shared;
using System.Text.Json;

namespace DirectoryService.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(params IEnumerable<Error> errors) : base(JsonSerializer.Serialize(errors))
    {

    }
}
