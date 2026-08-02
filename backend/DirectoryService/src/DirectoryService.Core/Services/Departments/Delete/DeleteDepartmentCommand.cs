using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.Delete;

public record DeleteDepartmentCommand(Guid Id) : ICommand
{
    public static implicit operator DeleteDepartmentCommand(Guid id) => new(id);
}
