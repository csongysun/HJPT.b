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
        public int Seeder { get; set; }
        public int Leecher { get; set; }

    }

    public class TorrentSubFile : Entity
    {
        public string Path { get; set; }
        public int Size { get; set; }
    }

    public class Peer : Entity
    {
        public string TorrentId { get; set; }
        public string PeerId { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public long Uploaded { get; set; }
        public long Downloaded { get; set; }
        public long ToGo { get; set; }
        public bool IsSeeder { get; set; }
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset LastAction { get; set; }
        public DateTimeOffset PrevAction { get; set; }
        public bool Connectable { get; set; }
        public string UserId { get; set; }
        public string Agent { get; set; }
        public int FinishEdat { get; set; }
        public long DownloadOffset { get; set; }
        public long UploadOffset { get; set; }
        public string PassKey { get; set; }
    }

}
