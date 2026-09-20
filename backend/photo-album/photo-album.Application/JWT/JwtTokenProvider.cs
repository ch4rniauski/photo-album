using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using photo_album.Application.Contracts.Jwt;
using photo_album.Domain.Entities;

namespace photo_album.Application.JWT;

internal sealed class JwtTokenProvider : ITokenProvider
{
    private readonly JwtSettings _settings;
    
    public JwtTokenProvider(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }
    
    public string GenerateAccessToken(UserEntity user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
