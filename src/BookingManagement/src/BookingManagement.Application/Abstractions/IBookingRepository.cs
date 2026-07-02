using BookingManagement.Application.Bookings;
using BookingManagement.Application.Common;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Abstractions;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> HasActiveOverlapAsync(
        string resourceId,
        DateTimeOffset startDateTimeUtc,
        DateTimeOffset endDateTimeUtc,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Booking>> GetForResourceAsync(
        GetBookingsQuery query,
        CancellationToken cancellationToken = default);
}