using BookingManagement.Domain.Enums;
using BookingManagement.Domain.Exceptions;

namespace BookingManagement.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }

    public string ResourceId { get; private set; } = null!;
    public string UserId { get; private set; } = null!;

    public DateTimeOffset StartDateTimeUtc { get; private set; }
    public DateTimeOffset EndDateTimeUtc { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }


    public Resource? Resource { get; private set; }
    public BookingUser? User { get; private set; }

    private Booking() { }

    private Booking(
        Guid id,
        string resourceId,
        string userId,
        DateTimeOffset startDateTimeUtc,
        DateTimeOffset endDateTimeUtc,
        DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
            throw new ArgumentException("Resource id is required.", nameof(resourceId));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        var normalizedStart = startDateTimeUtc.ToUniversalTime();
        var normalizedEnd = endDateTimeUtc.ToUniversalTime();

        if (normalizedStart >= normalizedEnd)
            throw new InvalidBookingTimeRangeException();

        Id = id;
        ResourceId = resourceId;
        UserId = userId;
        StartDateTimeUtc = normalizedStart;
        EndDateTimeUtc = normalizedEnd;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
        Status = BookingStatus.Active;
    }

    public static Booking Create(
        string resourceId,
        string userId,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        DateTimeOffset createdAtUtc)
    {
        return new Booking(
            Guid.NewGuid(),
            resourceId,
            userId,
            startDateTime,
            endDateTime,
            createdAtUtc);
    }

    public void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (Status == BookingStatus.Cancelled)
            throw new BookingAlreadyCancelledException(Id);

        Status = BookingStatus.Cancelled;
        CancelledAtUtc = cancelledAtUtc.ToUniversalTime();
    }

    public static bool PeriodsOverlap(
        DateTimeOffset firstStart,
        DateTimeOffset firstEnd,
        DateTimeOffset secondStart,
        DateTimeOffset secondEnd)
    {
        return firstStart < secondEnd && secondStart < firstEnd;
    }
}