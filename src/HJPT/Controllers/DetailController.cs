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

        private readonly ITorrentService _torrentService;
        public DetailController(ITorrentService torrentService)
        {
            _torrentService = torrentService;
        }

        public async Task<IActionResult> Upload([FromForm]UploadForm form)
        {
            if (form.torrent == null) return BadRequest();

            var torrent = await _torrentService.SetTorrent(form.torrent.OpenReadStream());

            await _torrentService.SaveToFile("Data/Torrents", torrent.Id);
            
            return Ok();
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownLoad(string id)
        {
            return File("Data/Torrents/111.torrent", "application/x-bittorrent");
        }



    }
}

