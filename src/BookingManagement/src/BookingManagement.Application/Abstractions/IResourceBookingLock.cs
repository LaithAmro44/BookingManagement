namespace BookingManagement.Application.Abstractions;

public interface IResourceBookingLock
{
    Task<T> ExecuteAsync<T>(
        string resourceId,
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default);
}