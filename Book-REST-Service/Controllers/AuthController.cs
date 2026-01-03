using BusinessLogic.Interfaces;
using DTOs;
using Book_REST_Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserControl _userControl;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(
            IUserControl userControl,
            JwtTokenService jwtTokenService)
        {
            _userControl = userControl;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email mangler");

            User? user = await _userControl.GetByEmail(dto.Email);

            if (user == null)
                return Unauthorized("Forkert login");

            string accessToken = _jwtTokenService.GenerateAccessToken(user);

            return Ok(new
            {
                accessToken,
                user = new
                {
                    user.UserId,
                    user.Email,
                    user.FirstName,
                    user.LastName
                }
            });
        }
    }
}
