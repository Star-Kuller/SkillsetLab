using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SkillsetLab.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly TokenManagement _tokenManagement;

        public JwtTokenProvider(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }

        public string GetToken(UserClaims userClaims)
        {
            var jwtToken = new JwtSecurityToken(
                issuer: _tokenManagement.Issuer,
                audience: _tokenManagement.Audience,
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, userClaims.Id.ToString()),
                    new Claim(ClaimTypes.Role, userClaims.Role),
                },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(_tokenManagement.Lifetime),
                signingCredentials: new SigningCredentials(_tokenManagement.SecurityKey, SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
