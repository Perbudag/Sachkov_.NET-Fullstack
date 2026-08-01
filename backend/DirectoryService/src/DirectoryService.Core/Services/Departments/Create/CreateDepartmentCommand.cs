using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.Create;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand<CreateDepartmentCommand, Guid>
{
    public static implicit operator CreateDepartmentCommand(CreateDepartmentRequest Request) => new(Request);
}
