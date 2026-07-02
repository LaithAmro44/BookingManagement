using BookingManagement.Application.Abstractions;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Tests.Fakes;

public class FakeResourceRepository : IResourceRepository
{
    private readonly bool _exists;

    public FakeResourceRepository(bool exists)
    {
        _exists = exists;
    }

    public Task<bool> ExistsAsync(
        string resourceId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_exists);
    }

    public Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}