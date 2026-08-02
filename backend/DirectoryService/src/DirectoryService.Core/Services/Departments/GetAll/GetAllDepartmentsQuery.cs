using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.GetAll;

public record GetAllDepartmentsQuery() : IQuery<GetAllDepartmentsQuery, DepartmentResponse[]>;