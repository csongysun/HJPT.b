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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using HJPT.Options;

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private SiteOptions _siteOptions;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IOptions<SiteOptions> setting)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _siteOptions = setting.Value;

            _logger = loggerFactory.CreateLogger<AuthController>();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginForm form)
        {
            var result = await _signInManager.PasswordSignInAsync(form.UserName, form.Password, isPersistent: true, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var u = await _userManager.FindByNameAsync(form.UserName);
                u.LastIP = Request.Host.Host;
                await _userManager.UpdateAsync(u);
                _logger.LogInformation($"User ({form.UserName}) log in");
                return Json( await _userManager.FindByNameAsync(form.UserName));
            }

            return BadRequest("用户名或密码错误");
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody]SignUpForm form)
        {
            if (_siteOptions.SignUp == SignUpOption.Reject)
                return BadRequest();
            var user = new ApplicationUser { UserName = form.UserName, Email = form.Email, StuID = form.StuID, RegIP = Request.Host.Host };
            var task = _userManager.CreateAsync(user, form.Password);
            var result = await task;
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user);
                return Json( await _userManager.FindByNameAsync(user.UserName));
            }
            return BadRequest(task.Result.Errors);
        }

        [HttpPost("signup/invite")]
        [AllowAnonymous]
        public async Task<IActionResult> InviteSignUp([FromBody]SignUpForm form)
        {
            if (_siteOptions.SignUp != SignUpOption.Invite || form.InviteToken == null)
                return BadRequest();

            //valitade token

            return await SignUp(form);
        }


        [HttpGet("signout")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("get")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult( (IActionResult) Json(_siteOptions.SignUp));
        }

        [HttpGet("set")]
        public async Task<IActionResult> Set()
        {
            _siteOptions.SignUp = SignUpOption.Open;
            return await Task.FromResult((IActionResult)Json(_siteOptions.SignUp));
        }

    }
}
