namespace DirectoryService.Contracts.Departments;

public record DepartmentTreeNodeDto(Guid Id,
                                    string Name,
                                    string Slug,
                                    string Path,
                                    int Depth,
                                    bool HasChildren);