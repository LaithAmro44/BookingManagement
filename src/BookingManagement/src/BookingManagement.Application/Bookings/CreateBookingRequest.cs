namespace BookingManagement.Application.Bookings;

public sealed record CreateBookingRequest(
    string ResourceId,
    string UserId,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime);