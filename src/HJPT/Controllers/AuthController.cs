using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HJPT.Model;
using Microsoft.AspNetCore.Http.Authentication;
using HJPT.Common;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET api/values/5
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginForm user)
        {
            User xuser;
            try
            {
                xuser = _userRepository.Login(user);
            }
            catch (AuthenticationFailedException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

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
