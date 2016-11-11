
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HJPT.Models;
using HJPT.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Csys.Common;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HJPT.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class TorrentController : Controller
    {

        private readonly ITorrentManager _torrent;
        private readonly TopicManager _topic;
        public TorrentController(
            ITorrentManager torrentManager,
            TopicManager topic)
        {
            _torrent = torrentManager;
            _topic = topic;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]UploadModel form)
        {
            if (form.torrent == null) return BadRequest();

            var torrent = await _torrent.ReadTorrent(form.torrent.OpenReadStream());
            await _torrent.SaveToFile("Data/Torrents", torrent.TorrentFile, torrent.Id);

            return Ok();
        }

        [HttpGet("download/{id}")]
        public IActionResult DownLoad(string id)
        {
            string passKey = Request.Query["passkey"];
            if(string.IsNullOrEmpty(passKey) || string.IsNullOrEmpty(id)) 
                return BadRequest(new []{ErrorDescriber.ModelNotValid});
            var fs = _torrent.ReadTorrentFile(id, passKey);
            return File(fs, "application/x-bittorrent");
        }

        [HttpGet]
        public IActionResult Topics()
        {
            List<Topic> topics;
            try
            {
                topics = _topic.GetTopics(this.Request.Query);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(topics);
        }
    }
}

