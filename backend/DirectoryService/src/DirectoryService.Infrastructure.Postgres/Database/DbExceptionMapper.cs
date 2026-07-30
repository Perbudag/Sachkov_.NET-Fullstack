using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Database;

internal static class DbExceptionMapper
{
    public static Error Map(Exception exception, ILogger logger, string defaultMessage)
    {
        switch (exception)
        {
            case DbUpdateConcurrencyException concurrencyException:
                logger.LogError(concurrencyException, "Concurrent record modification conflict");
                return Error.Failure("database.conflict", "The record was modified or deleted by another user; please try again.");

            case DbUpdateException { InnerException: PostgresException postgresException }:
                return MapPostgresException(postgresException, logger, defaultMessage);

            case PostgresException postgresException:
                return MapPostgresException(postgresException, logger, defaultMessage);

            default:
                logger.LogError(exception, "{Message}", defaultMessage);
                return Error.Failure("database.failure", defaultMessage);
        }
    }

    private static Error MapPostgresException(PostgresException exception, ILogger logger, string defaultMessage)
    {
        switch (exception.SqlState)
        {
            case PostgresErrorCodes.UniqueViolation:
                logger.LogWarning(exception, "Uniqueness violation during save. Constraint: {Constraint}", exception.ConstraintName);
                return Error.Conflict("database.unique", "A record with this data already exists.");

            case PostgresErrorCodes.ForeignKeyViolation:
                logger.LogWarning(exception, "Foreign key violation during save. Constraint: {Constraint}", exception.ConstraintName);
                return Error.Conflict("database.foreign.key", "The operation is not possible due to related records.");

            default:
                logger.LogError(exception, "{Message}. SqlState: {SqlState}", defaultMessage, exception.SqlState);
                return Error.Failure("database.failure", defaultMessage);
        }
    }
}
