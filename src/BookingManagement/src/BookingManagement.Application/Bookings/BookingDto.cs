using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;

namespace BookingManagement.Application.Bookings;

public sealed record BookingDto(
    Guid Id,
    string ResourceId,
    string ResourceName,
    string UserId,
    string UserDisplayName,
    DateTimeOffset StartDateTimeUtc,
    DateTimeOffset EndDateTimeUtc,
    BookingStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? CancelledAtUtc)
{
    public static BookingDto FromEntity(Booking booking)
    {
        return new BookingDto(
            booking.Id,
            booking.ResourceId,
            booking.Resource?.Name ?? booking.ResourceId,
            booking.UserId,
            booking.User?.DisplayName ?? booking.UserId,
            booking.StartDateTimeUtc,
            booking.EndDateTimeUtc,
            booking.Status,
            booking.CreatedAtUtc,
            booking.CancelledAtUtc);
    }
}