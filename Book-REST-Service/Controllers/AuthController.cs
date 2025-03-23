using DTOs;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using System.Threading.Tasks;

namespace Book_REST_Service.Controllers
{
    [ApiController]
    [Route("api/auth")] 
    public class AuthController : ControllerBase
    {
        private readonly IUserControl _userControl;

        public AuthController(IUserControl userControl)
        {
            _userControl = userControl;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                return BadRequest("Email is required");

            var (user, token) = await _userControl.LoginAsync(loginRequest.Email);

            if (user == null)
                return Unauthorized("Bruger ikke fundet");

            return Ok(new { user, token });
        }
    }
}
