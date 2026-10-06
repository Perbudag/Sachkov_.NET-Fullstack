using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Departments.RemoveLocation;

public record RemoveLocationInDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand
{
    public static implicit operator RemoveLocationInDepartmentCommand((Guid, Guid) args) => new(args.Item1, args.Item2);
}