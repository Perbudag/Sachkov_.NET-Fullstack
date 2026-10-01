using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Services.Departments.ChangeParent;

public record ChangeDepartmentParentCommand(Guid Id, PutDepartmentParentRequest Request) : ICommand<ChangeDepartmentParentCommand, PutDepartmentParentDto>
{
    public static implicit operator ChangeDepartmentParentCommand((Guid, PutDepartmentParentRequest) args) =>
        new ChangeDepartmentParentCommand(args.Item1, args.Item2);
}
