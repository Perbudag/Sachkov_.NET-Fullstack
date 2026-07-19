using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Entities;

public class DepartmentPosition
{
    private DepartmentPosition(Guid departmentId, Guid positionId)
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;

        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition() { }


    public Guid Id { get; }

    public Guid DepartmentId { get; }
    public Guid PositionId { get; }

    public DateTime CreatedAt { get; }


    public static Result<DepartmentPosition, Failure> Create(Guid departmentId, Guid positionId)
    {
        var errors = new List<Error>();

        if (departmentId == Guid.Empty)
        {
            errors.Add(Error.Validation("departmentId не должен быть пустым.", "department.position.validation.error", nameof(departmentId)));
        }

        if (positionId == Guid.Empty)
        {
            errors.Add(Error.Validation("positionId не должен быть пустым.", "department.position.validation.error", nameof(positionId)));
        }

        if (errors.Count > 0)
            return new Failure(errors);

        return new DepartmentPosition(departmentId, positionId);
    }
}