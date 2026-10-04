using CSharpFunctionalExtensions;
using DirectoryService.Domain.Errors;
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
            SharedErrors.IsRequired(nameof(departmentId), "department.position.validation.error");
        }

        if (positionId == Guid.Empty)
        {
            SharedErrors.IsRequired(nameof(positionId), "department.position.validation.error");
        }

        if (errors.Count > 0)
            return new Failure(errors);

        return new DepartmentPosition(departmentId, positionId);
    }
}