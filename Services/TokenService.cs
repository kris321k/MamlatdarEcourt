using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MamlatdarEcourt.Models;

namespace MamlatdarEcourt.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException("User email is required to generate JWT.");
            }

            if (string.IsNullOrWhiteSpace(user.Id))
            {
                throw new InvalidOperationException("User ID is missing.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _config.GetValue<string>("Jwt:Key")
                ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

            var issuer = _config.GetValue<string>("Jwt:Issuer")
                ?? throw new InvalidOperationException("JWT Issuer is missing.");

            var audience = _config.GetValue<string>("Jwt:Audience")
                ?? throw new InvalidOperationException("JWT Audience is missing.");

            var duration = _config.GetValue<int?>("Jwt:DurationInMinutes")
                ?? throw new InvalidOperationException("JWT Duration is missing.");

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: credentials
            );

            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
