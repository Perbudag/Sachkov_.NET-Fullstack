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


    public static DepartmentPosition Create(Guid departmentId, Guid positionId)
    {
        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException("departmentId не должен быть пустым.", nameof(departmentId));
        }

        if (positionId == Guid.Empty)
        {
            throw new ArgumentException("positionId не должен быть пустым.", nameof(positionId));
        }

        return new DepartmentPosition(departmentId, positionId);
    }
}