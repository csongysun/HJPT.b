using BencodeNET;
using BencodeNET.Objects;
using HJPT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJPT.Services
{
    public interface ITorrentService
    {
        Dictionary<string, object> DecodeTorrent(Stream stream);
    }

    public class TorrentService
    {
        private TorrentFile torrentObj;

        public Task<Torrent> SetTorrent(Stream stream)
        {
            var torrentObj = Bencode.DecodeTorrentFile(stream);
            var torrent = new Torrent();

            IBObject files;
            int size = 0;
            if(torrentObj.Info.TryGetValue(new BString("files"), out files))
            {
                torrent.Files = new List<TorrentSubFile>();
                foreach (BDictionary file in files as BList)
                {
                    string path = "";
                    if (file["path"] is BList)
                    {
                        foreach(BString p in file["path"] as BList)
                        {
                            path += p + "/";
                        }
                        path = path.TrimEnd('/');
                    }
                    size += file["length"] as BNumber;
                    torrent.Files.Add(new TorrentSubFile
                    {
                        Path = path,
                        Size = file["length"] as BNumber,
                    });
                }
            }
            else
            {
                size = torrentObj.Info["length"] as BNumber;
                torrent.Files.Add(new TorrentSubFile
                {
                    Path = (torrentObj.Info["name"] as BString).ToString(),
                    Size = torrentObj.Info["length"] as BNumber,
                });
            }

            torrent.Size = size;
            torrent.InfoHash = torrentObj.CalculateInfoHash();

            return Task.FromResult(torrent);
        }

    }

}
