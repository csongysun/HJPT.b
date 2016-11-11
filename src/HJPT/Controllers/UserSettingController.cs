using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Csys.Identity;

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserSettingController : Controller
    {
        private UserManager _userManager;
        public UserSettingController(
            UserManager userManager
            )
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassKey()
        {
            var result = await _userManager.UpdatePassKeyAsync(User.Identity.Name);
            if(result.Succeeded)
                return Ok();
            return NotFound(result.Errors);
        }

    }
}
