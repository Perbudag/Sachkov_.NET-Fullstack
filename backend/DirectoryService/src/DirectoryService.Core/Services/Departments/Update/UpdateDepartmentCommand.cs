using DirectoryService.Contracts.Departments;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Departments.Update;

public record UpdateDepartmentCommand(Guid Id, UpdateDepartmentRequest Request) : ICommand<UpdateDepartmentCommand, DepartmentDto>
{
    public static implicit operator UpdateDepartmentCommand((Guid, UpdateDepartmentRequest) args) => 
        new(args.Item1, args.Item2);
}
