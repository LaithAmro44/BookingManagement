using BookingManagement.Application.Common;

namespace BookingManagement.Application.Bookings;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<BookingDto>> GetForResourceAsync(
        GetBookingsQuery query,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);
}