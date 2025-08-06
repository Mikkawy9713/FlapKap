// WebApi/Controllers/TestController.cs
using Microsoft.AspNetCore.Mvc;

namespace FlapKapVendingMachine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API is running 🚀");
}
