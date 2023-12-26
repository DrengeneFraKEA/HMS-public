using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HMS.Utils
{
    public class JwtTokenGenerator
    {

        public JwtTokenGenerator() { }

        public string GenerateToken(string username, string role)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            var keyString = Environment.GetEnvironmentVariable("JWT_KEY").IsNullOrEmpty()
            ? config["JwtSettings:Key"]
            : Environment.GetEnvironmentVariable("JWT_KEY");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role) };

            var token = new JwtSecurityToken(
                issuer: config["JwtSettings:Issuer"] ??  Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: config["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       
    }
}
