using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Departments;

public record MoveDepartmentDto(Guid Id,
                                         Guid? ParentId,
                                         string Path,
                                         int Depth,
                                         DateTime UpdatedAt);
