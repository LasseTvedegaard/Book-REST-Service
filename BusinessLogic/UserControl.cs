using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogic
{
    public class UserControl : IUserControl
    {
        private readonly IUserAccess _userAccess;
        private readonly IConfiguration _configuration;

        public UserControl(IUserAccess userAccess, IConfiguration configuration)
        {
            _userAccess = userAccess;
            _configuration = configuration;
        }

        public async Task<bool> Create(User entity) => await _userAccess.Create(entity);

        public async Task<User> Get(string id) => await _userAccess.Get(id);

        public async Task<bool> Update(User entity) => await _userAccess.Update(entity);

        public async Task<User?> GetByEmail(string email) => await _userAccess.GetByEmail(email);

        // 👇 Login-funktionalitet + token
        public async Task<(User user, string token)> LoginAsync(string email)
        {
            var user = await _userAccess.GetByEmail(email);

            if (user == null)
                return (null, null);

            var token = GenerateJwtToken(user);
            return (user, token);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
