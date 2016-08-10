﻿using BencodeNET;
using BencodeNET.Objects;
using HJPT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace HJPT.Services
{
    public interface ITorrentService
    {
        Task<Torrent> SetTorrent(Stream stream);
        Task SaveToFile(string path, string filename);

    }

    public class TorrentService : ITorrentService
    {
        private TorrentFile torrentObj;

        public Task<Torrent> SetTorrent(Stream stream)
        {
            torrentObj = Bencode.DecodeTorrentFile(stream);
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

            return Task.FromResult(torrent);
        }

        public Task<MemoryStream> SaveToMemoryStream()
        {
            var stream = new MemoryStream();
            torrentObj.EncodeToStream(stream);
            return Task.FromResult(stream);
        }

        public async Task SaveToFile(string path, string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                torrentObj.EncodeToStream(fs);
                await fs.FlushAsync();
            }
        }

    }

}
