using BookingManagement.Application.Abstractions;

namespace BookingManagement.Application.Tests.Fakes;

public class PassThroughResourceBookingLock : IResourceBookingLock
{
    public Task<T> ExecuteAsync<T>(
        string resourceId,
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        return action(cancellationToken);
    }
}