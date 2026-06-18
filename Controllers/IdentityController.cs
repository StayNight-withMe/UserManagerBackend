using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Abstractions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IIdentityService identityService, ILogger<IdentityController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            _logger.LogInformation("Entering Register action with email: {Email}", dto.Email);
            var result = await _identityService.Register(dto);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _identityService.Login(dto);
            if (result.Success)
            {
                return Ok(new { Token = result.Token });
            }
            return Unauthorized(result.Message);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var success = await _identityService.VerifyEmail(email, token);
            if (success)
            {
                return Ok("Verified");
            }
            return BadRequest("Invalid token or email");
        }
    }
}

