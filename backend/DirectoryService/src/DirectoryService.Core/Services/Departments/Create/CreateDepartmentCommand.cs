using DirectoryService.Contracts.Departments;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Departments.Create;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand<CreateDepartmentCommand, Guid>
{
    public static implicit operator CreateDepartmentCommand(CreateDepartmentRequest Request) => new(Request);
}
