using HJPT.Models;
using HJPT.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HJPT.Services
{
    public interface ITokenProvider
    {
    }

    public class JwtProvider : ITokenProvider
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private HttpContext _context;
        public JwtProvider(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> GetToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    _jwtOptions.IssuedAt.ToString(),
                    ClaimValueTypes.DateTime),
                //new Claim(ClaimTypes.Role, user.Roles.First().)
            };
            return "";
        }

        internal HttpContext Context
        {
            get
            {
                var context = _context ?? new HttpContextAccessor()?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
            set
            {
                _context = value;
            }

        }
    }

}
