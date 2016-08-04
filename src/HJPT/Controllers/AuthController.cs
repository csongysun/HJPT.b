using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HJPT.Models;
using Microsoft.AspNetCore.Http.Authentication;
using HJPT.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using HJPT.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            //_logger = loggerFactory.CreateLogger<AuthController>();

        }

        // GET api/values/5
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginForm user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User ({user.Username}) log in");

                return Json(_userManager.FindByNameAsync(user.Username));
            }

            return BadRequest();
            

        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody]SignUpForm form)
        {
            var user = new ApplicationUser { UserName = form.Username, Email = form.Email, PasswordHash = form.Password, StuID = form.StuID };
            var result = await _userManager.CreateAsync(user);
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(_userManager.FindByNameAsync(user.UserName));
            }
            return new BadRequestResult();
        }

    }
}
