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

            _logger = loggerFactory.CreateLogger<AuthController>();

        }

        // GET api/values/5
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginForm user)
        {
            var task = _signInManager.PasswordSignInAsync(user.UserName, user.Password, isPersistent: true, lockoutOnFailure: true);
            var result = await task;
            if (result.Succeeded)
            {
                var u = await _userManager.FindByNameAsync(user.UserName);
              //  u.
                _logger.LogInformation($"User ({user.UserName}) log in");
                return Json( await _userManager.FindByNameAsync(user.UserName));
            }

            return new BadRequestObjectResult("用户名或密码错误");
            

        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody]SignUpForm form)
        {
            var user = new ApplicationUser { UserName = form.UserName, Email = form.Email, StuID = form.StuID };
            var task = _userManager.CreateAsync(user, form.Password);
            var result = await task;
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user);
                return Json( await _userManager.FindByNameAsync(user.UserName));
            }
            return new BadRequestObjectResult(task.Result.Errors);
        }

        [HttpPost("signout/{userName}")]
        public async Task<IActionResult> SignOut(string userName)
        {
            await _signInManager.SignOutAsync(userName);
            return Json(await _userManager.FindByNameAsync("sss"));
        }

    }
}
