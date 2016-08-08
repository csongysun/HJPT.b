using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HJPT.Models;
using System.IO;
using HJPT.Services;
using System.Security.Cryptography;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HJPT.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class DetailController : Controller
    {

        public async Task<IActionResult> Upload([FromForm]UploadForm form)
        {
            //var path = "Data/Torrents/123.torrent";
            //if (form.torrent == null) return BadRequest();

            //var result = _btds.DecodeTorrent(form.torrent.OpenReadStream());



            //using (FileStream fs = System.IO.File.Create(path))
            //{
            //    await form.torrent.CopyToAsync(fs);
            //    await fs.FlushAsync();
            //}

            //return Json(result);
            return Ok();
        }



    }
}

