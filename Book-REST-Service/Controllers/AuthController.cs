using DTOs;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using Model;

namespace Book_REST_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrWhiteSpace(registerRequest.FirstName) ||
                string.IsNullOrWhiteSpace(registerRequest.LastName))
            {
                return BadRequest("Alle felter er påkrævede");
            }

            var existingUser = await _userControl.GetByEmail(registerRequest.Email);
            if (existingUser != null)
            {
                return Conflict("Bruger findes allerede");
            }

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName
            };

            var success = await _userControl.Create(user);

            if (!success)
                return StatusCode(500, "Fejl under oprettelse af bruger");

            var (createdUser, token) = await _userControl.LoginAsync(user.Email);

            return Ok(new { user = createdUser, token });
        }
    }
}
