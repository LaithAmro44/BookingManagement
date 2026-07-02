using BookingManagement.Application.Abstractions;

namespace BookingManagement.Application.ReferenceData;

public sealed class ReferenceDataService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IBookingUserRepository _bookingUserRepository;

    public ReferenceDataService(
        IResourceRepository resourceRepository,
        IBookingUserRepository bookingUserRepository)
    {
        _resourceRepository = resourceRepository;
        _bookingUserRepository = bookingUserRepository;
    }

    public async Task<IReadOnlyList<ResourceDto>> GetResourcesAsync(
        CancellationToken cancellationToken = default)
    {
        var resources = await _resourceRepository.GetAllAsync(cancellationToken);

        return resources
            .OrderBy(x => x.Name)
            .Select(x => new ResourceDto(x.Id, x.Name))
            .ToList();
    }

    public async Task<IReadOnlyList<BookingUserDto>> GetUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _bookingUserRepository.GetAllAsync(cancellationToken);

        return users
            .OrderBy(x => x.DisplayName)
            .Select(x => new BookingUserDto(x.Id, x.DisplayName))
            .ToList();
    }
}