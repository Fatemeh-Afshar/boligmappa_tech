using Boligmappa.Api.Models;
using Boligmappa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boligmappa.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly IDocumentService _service;

    public PropertiesController(IDocumentService service)
    {
        _service = service;
    }

    /// <summary>List all documents belonging to a property.</summary>
    [HttpGet("{propertyId:guid}/documents")]
    [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Document>>> GetDocuments(Guid propertyId)
    {
        var documents = await _service.GetForPropertyAsync(propertyId);
        return Ok(documents);
    }
}
