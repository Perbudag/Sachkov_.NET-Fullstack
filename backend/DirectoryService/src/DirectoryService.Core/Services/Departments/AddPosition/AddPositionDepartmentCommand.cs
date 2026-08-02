using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.AddPosition;

public record AddPositionDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand
{
    public static implicit operator AddPositionDepartmentCommand((Guid, Guid) args) => new(args.Item1, args.Item2);
}