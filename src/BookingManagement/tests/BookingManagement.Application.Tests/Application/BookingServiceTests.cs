using BookingManagement.Application.Abstractions;
using BookingManagement.Application.Bookings;
using BookingManagement.Application.Common.Exceptions;
using BookingManagement.Application.Tests.Fakes;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Tests.Application;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenActiveOverlapExists_ThrowsBookingOverlapException()
    {
        var bookingRepository = new FakeBookingRepository
        {
            HasOverlapResult = true
        };

        var service = new BookingService(
            bookingRepository,
            new FakeResourceRepository(exists: true),
            new FakeBookingUserRepository(exists: true),
            new PassThroughResourceBookingLock(),
            new FakeUnitOfWork(),
            TimeProvider.System);

        var request = new CreateBookingRequest(
            "meeting-room-a",
            "user-001",
            new DateTimeOffset(2026, 7, 3, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero));

        await Assert.ThrowsAsync<BookingOverlapException>(() =>
            service.CreateAsync(request));
    }
}