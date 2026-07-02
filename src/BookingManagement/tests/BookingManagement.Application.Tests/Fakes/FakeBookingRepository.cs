using BookingManagement.Application.Abstractions;
using BookingManagement.Application.Bookings;
using BookingManagement.Application.Common;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;

namespace BookingManagement.Application.Tests.Fakes;

public class FakeBookingRepository : IBookingRepository
{
    public bool HasOverlapResult { get; set; }

    public List<Booking> AddedBookings { get; } = [];

    public Task<bool> HasActiveOverlapAsync(
        string resourceId,
        DateTimeOffset startDateTimeUtc,
        DateTimeOffset endDateTimeUtc,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HasOverlapResult);
    }

    public Task AddAsync(
        Booking booking,
        CancellationToken cancellationToken = default)
    {
        AddedBookings.Add(booking);

        return Task.CompletedTask;
    }

    public Task<Booking?> GetByIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = AddedBookings.FirstOrDefault(x => x.Id == bookingId);

        return Task.FromResult(booking);
    }

    public async Task<PagedResult<Booking>> GetForResourceAsync(GetBookingsQuery query, CancellationToken cancellationToken = default)
    {
        var bookings = AddedBookings.AsEnumerable();

        if (!query.IncludeCancelled)
        {
            bookings = bookings.Where(x => x.Status == BookingStatus.Active);
        }

        if (query.FromUtc.HasValue)
        {
            bookings = bookings.Where(x => x.EndDateTimeUtc > query.FromUtc.Value);
        }

        if (query.ToUtc.HasValue)
        {
            bookings = bookings.Where(x => x.StartDateTimeUtc < query.ToUtc.Value);
        }

        var items = bookings
            .OrderBy(x => x.StartDateTimeUtc)
            .ThenBy(x => x.EndDateTimeUtc)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagedResult<Booking>(
            items,
            query.PageNumber,
            query.PageSize,
            AddedBookings.Count);
    }
}