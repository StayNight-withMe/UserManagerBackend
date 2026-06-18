using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Abstractions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpPost("block")]
        public async Task<IActionResult> Block([FromBody] IEnumerable<Guid> userIds)
        {
            await _userService.BlockUsers(userIds);
            return Ok();
        }

        [HttpPost("unblock")]
        public async Task<IActionResult> Unblock([FromBody] IEnumerable<Guid> userIds)
        {
            await _userService.UnblockUsers(userIds);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] IEnumerable<Guid> userIds)
        {
            await _userService.DeleteUsers(userIds);
            return Ok();
        }

        [HttpDelete("unverified")]
        public async Task<IActionResult> DeleteUnverified()
        {
            await _userService.DeleteUnverifiedUsers();
            return Ok();
        }
    }
}

