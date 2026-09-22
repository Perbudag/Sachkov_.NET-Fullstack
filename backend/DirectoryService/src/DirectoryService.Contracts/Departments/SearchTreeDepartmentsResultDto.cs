namespace DirectoryService.Contracts.Departments;

public record SearchTreeDepartmentsResultDto(
    DepartmentTreeNodeDto Node,
    IEnumerable<DepartmentTreeNodeDto> Ancestors);