namespace BookingManagement.Application.Bookings;

public sealed class GetBookingsQuery
{
    public string ResourceId { get; init; } = string.Empty;
    public DateTimeOffset? FromUtc { get; init; }
    public DateTimeOffset? ToUtc { get; init; }
    public bool IncludeCancelled { get; init; } = true;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}