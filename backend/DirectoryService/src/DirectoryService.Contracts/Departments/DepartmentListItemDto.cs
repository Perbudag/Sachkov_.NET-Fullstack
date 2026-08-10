namespace DirectoryService.Contracts.Departments;

public record DepartmentListItemDto(Guid Id,
                                    string Name,
                                    string Slug,
                                    DateTime CreatedAt);