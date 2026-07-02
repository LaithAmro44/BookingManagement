using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Abstractions;

public interface IBookingUserRepository
{
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingUser>> GetAllAsync(
        CancellationToken cancellationToken = default);
}