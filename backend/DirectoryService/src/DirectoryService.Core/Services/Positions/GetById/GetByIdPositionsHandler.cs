using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Positions.GetById;

internal class GetByIdPositionsHandler : IQueryHandler<PositionDto, GetByIdPositionsQuery>
{
    private readonly IReadDbContext _context;

    public GetByIdPositionsHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PositionDto, Failure>> HandleAsync(GetByIdPositionsQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
            return Errors.SharedErrors.IsRequired("Id", "positions.validation.error").ToFailure();

        var response = await _context.PositionsRead
            .Where(p => p.Id == query.Id)
            .Select(p => new PositionDto(
                Id: p.Id,
                Name: p.Name.ToString())
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (response == null)
            return Errors.PositionsErrors.NotFoud().ToFailure();

        return response;
    }
}
