using BookingManagement.Application.Abstractions;
using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.Persistence.Repositories;

public sealed class BookingUserRepository : IBookingUserRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingUserRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return _dbContext.BookingUsers.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BookingUser>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.BookingUsers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}