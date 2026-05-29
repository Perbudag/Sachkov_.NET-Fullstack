namespace DirectoryService.Contracts.Departments;

public record UpdateDepartmentRequest(Guid Id,
                                      string Name,
                                      string Slug,
                                      Guid? ParentId);