using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Departments.RemovePosition;

public record RemovePositionDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand
{
    public static implicit operator RemovePositionDepartmentCommand((Guid, Guid) args) => new(args.Item1, args.Item2);
}
