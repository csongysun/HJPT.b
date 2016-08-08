using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Models
{
    public class Torrent : Entity
    {
        public string InfoHash { get; set; }
        public string Name { get; set; }
        public Promotion Promotion { get; set; }
        public int Size { get; set; }
        public string DName { get; set; }
        public List<TorrentFile> Files { get; set; }
    }

    public class TorrentFile : Entity
    {
        public string Path { get; set; }
        public int Size { get; set; }
    }

}
