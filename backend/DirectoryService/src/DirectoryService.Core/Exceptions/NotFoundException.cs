using Shared;
using System.Text.Json;

namespace DirectoryService.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(IEnumerable<Error> errors) : base(JsonSerializer.Serialize(errors))
    {

    }
}
