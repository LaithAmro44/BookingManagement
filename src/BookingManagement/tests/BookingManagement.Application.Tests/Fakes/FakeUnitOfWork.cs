using BookingManagement.Application.Abstractions;

namespace BookingManagement.Application.Tests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;

        return Task.FromResult(1);
    }
}