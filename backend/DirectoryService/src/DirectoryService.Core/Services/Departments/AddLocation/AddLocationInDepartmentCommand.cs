using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.AddLocation;

public record AddLocationInDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand
{
    public static implicit operator AddLocationInDepartmentCommand((Guid, Guid) args) => 
        new(args.Item1, args.Item2);
}