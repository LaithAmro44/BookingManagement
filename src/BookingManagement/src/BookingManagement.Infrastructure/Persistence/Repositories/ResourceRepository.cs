using BookingManagement.Application.Abstractions;
using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.Persistence.Repositories;

public sealed class ResourceRepository : IResourceRepository
{
    private readonly BookingDbContext _dbContext;

    public ResourceRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Resources.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Resources
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}