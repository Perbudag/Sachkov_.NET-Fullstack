using Shared;
using System.Text.Json;

namespace DirectoryService.Core.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(params IEnumerable<Error> errors) : base(JsonSerializer.Serialize(errors))
    {
        
    }
}
