namespace DirectoryService.Contracts.Departments;

public record PutDepartmentParentRequest(Guid? ParentId = null);