using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Positions.GetAll;

internal class GetAllPositionsHandler : IQueryHandler<PositionResponse[], GetAllPositionsQuery>
{
    private readonly IReadDbContext _context;

    public GetAllPositionsHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PositionResponse[], Failure>> HandleAsync(GetAllPositionsQuery query, CancellationToken cancellationToken)
    {
        var responses = await _context.PositionsRead
            .Select(p => new PositionResponse(
                Id: p.Id,
                Name: p.Name.ToString())
            )
            .ToArrayAsync(cancellationToken);

        return responses;
    }
}
