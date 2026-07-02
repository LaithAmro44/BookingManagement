using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Exceptions;

namespace BookingManagement.Application.Tests.Domain;

public class BookingTests
{
    [Fact]
    public void PeriodsOverlap_WhenOneBookingEndsExactlyAtTheOtherStart_ReturnsFalse()
    {
        var firstStart = new DateTimeOffset(2026, 7, 3, 9, 0, 0, TimeSpan.Zero);
        var firstEnd = new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);
        var secondStart = new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);
        var secondEnd = new DateTimeOffset(2026, 7, 3, 11, 0, 0, TimeSpan.Zero);

        var overlap = Booking.PeriodsOverlap(
            firstStart,
            firstEnd,
            secondStart,
            secondEnd);

        Assert.False(overlap);
    }

    [Fact]
    public void Create_WhenStartIsNotBeforeEnd_ThrowsException()
    {
        var start = new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);

        Assert.Throws<InvalidBookingTimeRangeException>(() =>
            Booking.Create(
                "meeting-room-a",
                "user-001",
                start,
                end,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Cancel_WhenBookingIsActive_MarksItCancelled()
    {
        var booking = Booking.Create(
            "meeting-room-a",
            "user-001",
            new DateTimeOffset(2026, 7, 3, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            DateTimeOffset.UtcNow);

        var cancelledAt = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        booking.Cancel(cancelledAt);

        Assert.Equal(BookingManagement.Domain.Enums.BookingStatus.Cancelled, booking.Status);
        Assert.Equal(cancelledAt, booking.CancelledAtUtc);
    }
}