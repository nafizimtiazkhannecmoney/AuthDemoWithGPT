using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Authdemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Everyone can access this.");
        }

        [Authorize]
        [HttpGet("private")]
        public IActionResult Private()
        {
            _logger.LogInformation("Accessing private endpoint by user: {User}", User.Identity?.Name);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                UserId = userId,
                Username = username,
                Role = role,
                Department = User.FindFirst("Department")?.Value
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Department = User.FindFirst("Department")?.Value

            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Welcome Admin!");
        }

        [Authorize(Policy = "FinanceOnly")]
        [HttpGet("finance")]
        public IActionResult Finance()
        {
            return Ok("Welcome Finance Department!");
        }

        [Authorize(Policy = "FinanceManager")]
        [HttpGet("financeManagerOnly")]
        public IActionResult FinanceManager()
        {
            return Ok("Welcome Finance Manager!");
        }
    }
}
