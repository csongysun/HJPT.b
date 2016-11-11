using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HJPT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Csys.Identity
{
    public interface ITokenProvider
    {
        string GenerateToken(User user, out DateTime expireTime);
        string GenerateRefreshToken(User user, out DateTime expireTime);
        ClaimsPrincipal ValidateRefreshToken(string token);
    }

    public class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenProvider(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }

        public string GenerateToken(User user, out DateTime expireTime)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim("Id", user.Id),
            };
            expireTime = DateTime.Now.AddMinutes(_jwtOptions.Expiration);
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expireTime,
                signingCredentials:
                new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken(User user, out DateTime expireTime)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim("Id", user.Id),
            };
            expireTime = DateTime.Now.AddMinutes(_jwtOptions.RefreshTokenExpiration);
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expireTime,
                signingCredentials:
                new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public ClaimsPrincipal ValidateRefreshToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)),

                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = handler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}
