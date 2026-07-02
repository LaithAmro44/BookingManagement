using System.Data;
using BookingManagement.Application.Abstractions;
using BookingManagement.Application.Common.Exceptions;
using BookingManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingManagement.Infrastructure.Concurrency;

public sealed class SqlServerResourceBookingLock : IResourceBookingLock
{
    private const int LockTimeoutMilliseconds = 5_000;
    private readonly BookingDbContext _dbContext;

    public SqlServerResourceBookingLock(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteAsync<T>(
        string resourceId,
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await AcquireExclusiveLockAsync(resourceId, cancellationToken);

        var result = await action(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return result;
    }

    private async Task AcquireExclusiveLockAsync(
        string resourceId,
        CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();

        await using var command = connection.CreateCommand();
        command.Transaction = _dbContext.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = """
            DECLARE @result INT;

            EXEC @result = sp_getapplock
                @Resource = @resource,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = @timeout;

            SELECT @result;
            """;

        var resourceParameter = command.CreateParameter();
        resourceParameter.ParameterName = "@resource";
        resourceParameter.Value = $"booking:{resourceId}";
        command.Parameters.Add(resourceParameter);

        var timeoutParameter = command.CreateParameter();
        timeoutParameter.ParameterName = "@timeout";
        timeoutParameter.Value = LockTimeoutMilliseconds;
        command.Parameters.Add(timeoutParameter);

        var lockResult = Convert.ToInt32(
            await command.ExecuteScalarAsync(cancellationToken));

        // Non-negative values mean the lock was granted.
        if (lockResult < 0)
        {
            throw new ResourceLockUnavailableException(resourceId);
        }
    }
}