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
        public List<TorrentSubFile> Files { get; set; }
    }

    public class TorrentSubFile : Entity
    {
        public string Path { get; set; }
        public int Size { get; set; }
    }

}
