using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MultipleAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentController : ControllerBase
{
    [Authorize(Policy = "OnlyCookieScheme")]
    [HttpGet("getWithCookie")]
    public IActionResult GetWithCookie()
    {
        var userName = HttpContext.User.Claims
            .Where(x => x.Type == ClaimTypes.Name)
            .Select(x => x.Value)
            .FirstOrDefault();

        return Content($"<p>Hello to Demo {userName}</p>");
    }
}