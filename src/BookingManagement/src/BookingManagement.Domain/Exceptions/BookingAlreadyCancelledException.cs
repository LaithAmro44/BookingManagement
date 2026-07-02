namespace BookingManagement.Domain.Exceptions;

public sealed class BookingAlreadyCancelledException : Exception
{
    public BookingAlreadyCancelledException(Guid bookingId)
        : base($"Booking '{bookingId}' is already cancelled.")
    {
    }
}