using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HJPT.Model;
using Microsoft.AspNetCore.Http.Authentication;
using HJPT.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using HJPT.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IUserRepository _userRepository;
        private ILogger _logger;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;

        public AuthController(IUserRepository userRepository, ILoggerFactory loggerFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            _userRepository = userRepository;

            _logger = loggerFactory.CreateLogger<AuthController>();
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        // GET api/values/5
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody]LoginForm user)
        {
            
            ApplicationUser xuser;
            try
            {
                xuser = await _userRepository.Login(user);
            }
            catch (AuthenticationFailedException e)
            {
                _logger.LogInformation($"Invalid username ({user.Username}) or password ({user.Password})");
                return new BadRequestObjectResult(e.Message);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, xuser.Username),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    _jwtOptions.IssuedAt.ToString(),
                    ClaimValueTypes.DateTime),
                new Claim(ClaimTypes.GroupSid, Enum.),
                //new Claim(ClaimTypes.)

            };

            return new JsonResult(xuser);
        }

        // POST api/auth/login
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody]SignUpForm user)
        {
            try
            {
                _userRepository.SignUp(user);
            }
            catch (AuthenticationFailedException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            return new OkResult();
        }

    }
}
