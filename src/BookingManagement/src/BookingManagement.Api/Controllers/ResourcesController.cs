using BookingManagement.Application.ReferenceData;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Api.Controllers;

[ApiController]
[Route("api/resources")]
public sealed class ResourcesController : ControllerBase
{
    private readonly ReferenceDataService _referenceDataService;

    public ResourcesController(ReferenceDataService referenceDataService)
    {
        _referenceDataService = referenceDataService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ResourceDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _referenceDataService.GetResourcesAsync(cancellationToken));
    }
}