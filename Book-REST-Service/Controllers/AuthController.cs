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
        private readonly RefreshTokenAccess _refreshTokenAccess;

        public AuthController(
            IUserControl userControl,
            RefreshTokenAccess refreshTokenAccess)
        {
            _userControl = userControl;
            _refreshTokenAccess = refreshTokenAccess;
        }

        // ---------------------------
        // LOGIN
        // ---------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                return BadRequest("Email is required");

            var (user, accessToken) = await _userControl.LoginAsync(loginRequest.Email);

            if (user == null)
                return Unauthorized("Bruger ikke fundet");

            // 🔑 Refresh token
            var refreshToken = TokenHelper.GenerateRefreshToken();
            var refreshHash = TokenHelper.Hash(refreshToken);

            await _refreshTokenAccess.InsertAsync(
                Guid.NewGuid(),
                Guid.Parse(user.UserId),
                refreshHash,
                DateTime.UtcNow.AddDays(14)
            );

            // 🍪 HttpOnly cookie
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(14)
            });

            return Ok(new { user, token = accessToken });
        }

        // ---------------------------
        // REGISTER
        // ---------------------------
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
                return Conflict("Bruger findes allerede");

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

            var (createdUser, accessToken) = await _userControl.LoginAsync(user.Email);

            // 🔑 Refresh token
            var refreshToken = TokenHelper.GenerateRefreshToken();
            var refreshHash = TokenHelper.Hash(refreshToken);

            await _refreshTokenAccess.InsertAsync(
                Guid.NewGuid(),
                Guid.Parse(createdUser.UserId),
                refreshHash,
                DateTime.UtcNow.AddDays(14)
            );

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(14)
            });

            return Ok(new { user = createdUser, token = accessToken });
        }

        // ---------------------------
        // REFRESH TOKEN
        // ---------------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
                return Unauthorized();

            var hash = TokenHelper.Hash(refreshToken);

            var userId = await _refreshTokenAccess.GetUserIdByValidTokenAsync(hash);
            if (userId == null)
                return Unauthorized();

            var user = await _userControl.Get(userId.Value.ToString());
            if (user == null)
                return Unauthorized();

            var newAccessToken = _userControl.GenerateJwtForUser(user);

            return Ok(new { token = newAccessToken });
        }


        // ---------------------------
        // LOGOUT
        // ---------------------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                await _refreshTokenAccess.RevokeAsync(TokenHelper.Hash(refreshToken));
            }

            Response.Cookies.Delete("refresh_token");
            return Ok();
        }
    }
}
