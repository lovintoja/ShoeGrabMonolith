using Microsoft.IdentityModel.Tokens;
using ShoeGrabMonolith.Database.Contexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ShoeGrabMonolith.Services;

public class JWTAuthenticationService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserContext _context;

    public JWTAuthenticationService(IConfiguration configuration, UserContext context)
    {
        _configuration = configuration;

    }

    public string GenerateToken(int id)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        // Add other claims as needed (e.g., roles)
    };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
