using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{

    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello, World!");
    }
}
