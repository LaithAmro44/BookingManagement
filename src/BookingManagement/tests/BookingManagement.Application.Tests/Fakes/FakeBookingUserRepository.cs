using BookingManagement.Application.Abstractions;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Tests.Fakes;

public class FakeBookingUserRepository : IBookingUserRepository
{
    private readonly bool _exists;

    public FakeBookingUserRepository(bool exists)
    {
        _exists = exists;
    }

    public Task<bool> ExistsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_exists);
    }

    public Task<IReadOnlyList<BookingUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}