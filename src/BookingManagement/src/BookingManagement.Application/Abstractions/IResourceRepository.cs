using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Abstractions;

public interface IResourceRepository
{
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default);
}