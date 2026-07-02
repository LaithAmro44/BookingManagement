namespace BookingManagement.Domain.Exceptions;

public sealed class InvalidBookingTimeRangeException : Exception
{
    public InvalidBookingTimeRangeException()
        : base("The booking start time must be earlier than its end time.")
    {
    }
}