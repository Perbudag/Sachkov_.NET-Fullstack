using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Departments;

public record PutDepartmentParentDto(Guid Id,
                                         Guid? ParentId,
                                         string Path,
                                         int Depth,
                                         DateTime UpdatedAt);
