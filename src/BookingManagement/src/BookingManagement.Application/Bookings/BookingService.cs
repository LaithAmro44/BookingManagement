using BookingManagement.Application.Abstractions;
using BookingManagement.Application.Common;
using BookingManagement.Application.Common.Exceptions;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Application.Bookings;

public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IBookingUserRepository _bookingUserRepository;
    private readonly IResourceBookingLock _resourceBookingLock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public BookingService(
        IBookingRepository bookingRepository,
        IResourceRepository resourceRepository,
        IBookingUserRepository bookingUserRepository,
        IResourceBookingLock resourceBookingLock,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _bookingRepository = bookingRepository;
        _resourceRepository = resourceRepository;
        _bookingUserRepository = bookingUserRepository;
        _resourceBookingLock = resourceBookingLock;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<BookingDto> CreateAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ResourceId))
            throw new ArgumentException("ResourceId is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("UserId is required.", nameof(request));

        var resourceId = request.ResourceId;
        var userId = request.UserId;

        var booking = Booking.Create(
            resourceId,
            userId,
            request.StartDateTime,
            request.EndDateTime,
            _timeProvider.GetUtcNow());

        if (!await _resourceRepository.ExistsAsync(resourceId, cancellationToken))
            throw new EntityNotFoundException("Resource", resourceId);

        if (!await _bookingUserRepository.ExistsAsync(userId, cancellationToken))
            throw new EntityNotFoundException("User", userId);

        return await _resourceBookingLock.ExecuteAsync(
            resourceId,
            async token =>
            {
                var hasOverlap = await _bookingRepository.HasActiveOverlapAsync(
                    resourceId,
                    booking.StartDateTimeUtc,
                    booking.EndDateTimeUtc,
                    token);

                if (hasOverlap)
                    throw new BookingOverlapException(resourceId);

                await _bookingRepository.AddAsync(booking, token);
                await _unitOfWork.SaveChangesAsync(token);

                return BookingDto.FromEntity(booking);
            },
            cancellationToken);
    }

    public async Task<PagedResult<BookingDto>> GetForResourceAsync(
        GetBookingsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.ResourceId))
            throw new ArgumentException("ResourceId is required.", nameof(query));

        if (query.PageNumber < 1)
            throw new ArgumentException("PageNumber must be at least 1.", nameof(query));

        if (query.PageSize is < 1 or > 100)
            throw new ArgumentException("PageSize must be between 1 and 100.", nameof(query));

        if (query.FromUtc.HasValue && query.ToUtc.HasValue && query.FromUtc >= query.ToUtc)
            throw new ArgumentException("FromUtc must be earlier than ToUtc.", nameof(query));

        var normalizedQuery = new GetBookingsQuery
        {
            ResourceId = query.ResourceId,
            FromUtc = query.FromUtc?.ToUniversalTime(),
            ToUtc = query.ToUtc?.ToUniversalTime(),
            IncludeCancelled = query.IncludeCancelled,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        if (!await _resourceRepository.ExistsAsync(normalizedQuery.ResourceId, cancellationToken))
            throw new EntityNotFoundException("Resource", normalizedQuery.ResourceId);

        var page = await _bookingRepository.GetForResourceAsync(normalizedQuery, cancellationToken);

        return new PagedResult<BookingDto>(
            page.Items.Select(BookingDto.FromEntity).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount);
    }

    public async Task CancelAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new EntityNotFoundException("Booking", bookingId);

        booking.Cancel(_timeProvider.GetUtcNow());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}