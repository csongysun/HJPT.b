
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HJPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HJPT.Options;
using Csys.Identity;
using Csys.Common;

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager _user;
        private ILogger _logger;
        private SiteOptions _siteOptions;

        public AuthController(
            UserManager user,
            ILoggerFactory loggerFactory,
            IOptions<SiteOptions> setting)
        {
            _user = user;
            _siteOptions = setting.Value;
            _logger = loggerFactory.CreateLogger<AuthController>();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new[] {ErrorDescriber.ModelNotValid});

            var result = await _user.PasswordSignInAsync(model.UserName, model.Password);
            if (result.Succeeded)
            {
                // var u = await _user.FindByEmailAsync(model.UserName);
                // u.LastIP = Request.Host.Host;
                // await _user.UpdateAsync(u);
                _logger.LogInformation($"User ({model.UserName}) log in");
                return Ok(result.Obj);
            }

            return BadRequest("用户名或密码错误");
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody]SignUpModel model)
        {
            if (_siteOptions.SignUp == SignUpOption.Reject || model == null || !ModelState.IsValid)
                return BadRequest(new[] {ErrorDescriber.ModelNotValid});

            var user = new User { UserName = model.UserName, Email = model.Email, StuID = model.StuID, RegIP = Request.Host.Host };
            var result = await _user.CreateAsync(model, Request.Host.Host);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("signup/invite")]
        [AllowAnonymous]
        public async Task<IActionResult> InviteSignUp([FromBody]SignUpModel form)
        {
            if (_siteOptions.SignUp != SignUpOption.Invite || form.InviteToken == null)
                return BadRequest();
            //Todo: valitade invite token
            return await SignUp(form);
        }

        [HttpGet("signout")]
        public IActionResult SignOut()
        {
            //Todo: signout
            _user.SignOutAsync(this.User.Identity.Name).RunSynchronously();
            return Ok();
        }

    }
}
