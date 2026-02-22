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
                throw new InvalidOperationException("User email is required.");

            if (string.IsNullOrWhiteSpace(user.Id))
                throw new InvalidOperationException("User ID is missing.");

            // ✅ FIXED CLAIMS
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),              // 🔥 IMPORTANT
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key missing.");

            var issuer = _config["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer missing.");

            var audience = _config["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience missing.");

            var duration = int.Parse(_config["Jwt:DurationInMinutes"]
                ?? throw new InvalidOperationException("JWT Duration missing."));

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