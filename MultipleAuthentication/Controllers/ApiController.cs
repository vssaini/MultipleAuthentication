using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MultipleAuthentication.Controllers;

// The idea for the ContentController is to (later) authorize only cookie-based requests.

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    [Authorize]
    [HttpGet("getWithAny")]
    public IActionResult GetWithAny()
    {
        return Ok(new { Message = $"Got user {GetUsername()} from Any" });
    }

    [Authorize(Policy = "OnlyAzureADUser")]
    [HttpGet("getWithAzureAd")]
    public IActionResult GetWithAzureAd()
    {
        return Ok(new { Message = "Welcome Azure AD request" });
    }

    [Authorize(Policy = "OnlyAWSUser")]
    [HttpGet("getWithAws")]
    public IActionResult GetWithAws()
    {
        return Ok(new { Message = "Welcome AWS request" });
    }

    private string? GetUsername()
    {
        return HttpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Name)
            .Select(x => x.Value)
            .FirstOrDefault();
    }
}