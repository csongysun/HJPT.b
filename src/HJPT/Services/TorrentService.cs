using BencodeNET;
using BencodeNET.Objects;
using HJPT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Csys.Common;

namespace HJPT.Services
{
    public interface ITorrentManager
    {
        Task<Torrent> ReadTorrent(Stream stream);
        Task SaveToFile(string path, TorrentFile torrentFile, string filename);
        Stream ReadTorrentFile(string torrentId, string passKey);

    }

    public class TorrentManager : ITorrentManager
    {


        public Task<Torrent> ReadTorrent(Stream stream)
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

            torrentObj.Announce = "http://localhost:2235/api/announce/";
            torrent.Size = size;
            torrent.InfoHash = torrentObj.CalculateInfoHash();
            torrent.TorrentFile = torrentObj;

            return Task.FromResult(torrent);
        }

        public Stream ReadTorrentFile(string torrentId, string passKey){
            TorrentFile tf = Bencode.DecodeTorrentFile($"Data/Torrents/{torrentId}.torrent");
            tf.Announce += $"?passkey={passKey}";
            return tf.EncodeToStream(new MemoryStream());
        }

        public async Task SaveToFile(string path, TorrentFile torrentFile, string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                torrentFile.EncodeToStream(fs);
                await fs.FlushAsync();
            }
        }

        public TaskResult FindTorrentByID(string id)
        {
            return EntityResult.EntityNotFound;
        }
    }

}
