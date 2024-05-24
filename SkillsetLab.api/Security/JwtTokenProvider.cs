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
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Sid, userClaims.Id.ToString()),
                    new (ClaimTypes.Role, userClaims.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(_tokenManagement.Lifetime),
                SigningCredentials = new SigningCredentials(_tokenManagement.SecurityKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _tokenManagement.Issuer,
                Audience = _tokenManagement.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
