namespace BookingManagement.Application.Common.Exceptions;

public sealed class BookingOverlapException : Exception
{
    public BookingOverlapException(string resourceId)
        : base($"Resource '{resourceId}' is already booked during the selected time range.")
    {
    }
}