namespace BookingManagement.Application.Common.Exceptions;

public sealed class ResourceLockUnavailableException : Exception
{
    public ResourceLockUnavailableException(string resourceId)
        : base($"Could not acquire the booking lock for resource '{resourceId}'. Please retry.")
    {
    }
}