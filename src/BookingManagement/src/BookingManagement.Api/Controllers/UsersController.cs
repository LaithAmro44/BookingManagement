using BookingManagement.Application.ReferenceData;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ReferenceDataService _referenceDataService;

    public UsersController(ReferenceDataService referenceDataService)
    {
        _referenceDataService = referenceDataService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingUserDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _referenceDataService.GetUsersAsync(cancellationToken));
    }
}