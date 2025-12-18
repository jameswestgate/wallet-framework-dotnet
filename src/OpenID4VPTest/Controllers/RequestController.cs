using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OpenID4VPTest.Models;
using OpenID4VPTest.Services;

namespace OpenID4VPTest.Controllers;

[ApiController]
public class RequestController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    [HttpGet("api/connect")]
    public IActionResult Connect()
    {
        return Ok(new { message = "success" });
    }

    [HttpPost("api/getRequest")]
    public IActionResult GetRequest([FromBody] GetRequestInput request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Invalid request body" });
        }

        if (string.IsNullOrWhiteSpace(request.Doctype))
        {
            return BadRequest(new { error = "Doctype is required" });
        }

        if (!request.Attrs.Any())
        {
            return BadRequest(new { error = "At least one attribute must be specified" });
        }

        var checkedAttrs = request.GetCheckedAttributes().ToList();
        if (!checkedAttrs.Any())
        {
            return BadRequest(new { error = "At least one attribute must be checked" });
        }

        var response = DcApiRequestBuilder.Build(request);
        return Ok(response);
    }

    //Add validateResponse endpoint 
    [HttpPost("api/validateResponse")]
    public IActionResult ValidateResponse([FromBody] JsonElement response)
    {
        // Process the JSON response
        return Ok(new { message = "Response received", data = response });
    }
}
