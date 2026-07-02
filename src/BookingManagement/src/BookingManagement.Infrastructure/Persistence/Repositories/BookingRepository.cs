using BookingManagement.Application.Abstractions;
using BookingManagement.Application.Bookings;
using BookingManagement.Application.Common;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> HasActiveOverlapAsync(
        string resourceId,
        DateTimeOffset startDateTimeUtc,
        DateTimeOffset endDateTimeUtc,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings
            .AsNoTracking()
            .AnyAsync(x =>
                x.ResourceId == resourceId &&
                x.Status == BookingStatus.Active &&
                x.StartDateTimeUtc < endDateTimeUtc &&
                startDateTimeUtc < x.EndDateTimeUtc,
                cancellationToken);
    }

    public async Task<PagedResult<Booking>> GetForResourceAsync(
        GetBookingsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Booking> bookings = _dbContext.Bookings
            .AsNoTracking()
            .Include(x => x.Resource)
            .Include(x => x.User)
            .Where(x => x.ResourceId == query.ResourceId);

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

        var totalCount = await bookings.CountAsync(cancellationToken);

        var items = await bookings
            .OrderBy(x => x.StartDateTimeUtc)
            .ThenBy(x => x.EndDateTimeUtc)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Booking>(
            items,
            query.PageNumber,
            query.PageSize,
            totalCount);
    }
}