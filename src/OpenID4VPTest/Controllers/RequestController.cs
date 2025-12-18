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

    [HttpPost("api/validateResponse")]
    public IActionResult ValidateResponse([FromBody] ValidateRequestInput request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Invalid request body" });
        }

        if (request.Data?.VpToken == null || !request.Data.VpToken.Any())
        {
            return BadRequest(new { error = "VP token is required" });
        }

        if (request.State == null)
        {
            return BadRequest(new { error = "State is required for validation" });
        }

        try
        {
            var response = DcApiResponseValidator.Validate(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Validation failed: {ex.Message}" });
        }
    }
}
