using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Controllers
{
    [Route("api")]
    [AllowAnonymous]
    public class AnnounceController : Controller
    {
        private TestBase test = new TestBase();

        public AnnounceController()
        {
            
        }

        [HttpGet("announce")]
        public async Task<IActionResult> Announce(string paramstr)
        {
            test.Write(new
            {
                
                Request.ContentType,
                Request.Headers,
                Request.Path,
                Request.Protocol,
                Request.Query,
                Request.QueryString,
                Request.Scheme,
            });

            return Ok();
        }

    }
}
