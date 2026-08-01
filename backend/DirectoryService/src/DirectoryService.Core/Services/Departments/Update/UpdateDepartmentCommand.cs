using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.Update;

public record UpdateDepartmentCommand(Guid Id, UpdateDepartmentRequest Request) : ICommand<UpdateDepartmentCommand, DepartmentResponse>
{
    public static implicit operator UpdateDepartmentCommand((Guid, UpdateDepartmentRequest) args) => 
        new(args.Item1, args.Item2);
}
