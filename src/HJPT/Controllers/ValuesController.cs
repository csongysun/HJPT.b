using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HJPT.Models;
using Microsoft.AspNetCore.Identity;

namespace HJPT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserSettingController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public UserSettingController(
            UserManager<ApplicationUser> userManager
            )
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassKey()
        {
            var user = await _userManager.GetUserAsync(User);
            user.PassKey = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);
            return Ok();
        }

    }
}
