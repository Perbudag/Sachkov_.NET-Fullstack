using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Services.Departments.ChangeParent;

public record MoveDepartmentCommand(Guid Id, MoveDepartmentRequest Request) : ICommand<MoveDepartmentCommand, MoveDepartmentDto>
{
    public static implicit operator MoveDepartmentCommand((Guid, MoveDepartmentRequest) args) =>
        new MoveDepartmentCommand(args.Item1, args.Item2);
}
