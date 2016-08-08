using HJPT.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HJPT.Services
{

    public class TorrentServiceTest : TestBase
    {

        [Theory]
        [InlineData("Data/Torrents/111.torrent")]
        public void SetTorrentTest(string path)
        {
            var ts = new TorrentService();

            var fs = new FileStream(path, FileMode.Open);

            Write(ts.SetTorrent(fs));
        }

    }


}
