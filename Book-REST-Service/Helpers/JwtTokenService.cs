using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Book_REST_Service.Helpers
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User is null in JWT generation");

            // Læs config
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            // 🔴 HÅRDE checks – så vi ALDRIG får skjulte 500-fejl
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new Exception("JWT configuration missing: Jwt:Key (check Azure App Setting: Jwt__Key)");

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new Exception("JWT configuration missing: Jwt:Issuer (check Azure App Setting: Jwt__Issuer)");

            if (string.IsNullOrWhiteSpace(jwtAudience))
                throw new Exception("JWT configuration missing: Jwt:Audience (check Azure App Setting: Jwt__Audience)");

            // Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
