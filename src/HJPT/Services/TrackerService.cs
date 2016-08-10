using BencodeNET.Objects;
using HJPT.Data;
using HJPT.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HJPT.Services
{
    public interface ITrackService
    {
        Task Accept(HttpRequest req);
    }

    public class TrackService : ITrackService
    {
        private readonly UserManager _userManager;
        private readonly ApplicationDbContext _dbContext;
        public TrackService(UserManager userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task Accept(HttpRequest req)
        {
            //todo: check agent client 
            

            var query = req.Query;
            if (!(query.ContainsKey("info_hash") &&
                query.ContainsKey("passkey") &&
                query.ContainsKey("peer_id") &&
                query.ContainsKey("port") &&
                query.ContainsKey("downloaded") &&
                query.ContainsKey("uploaded") &&
                query.ContainsKey("left") &&
                query["passkey"].ToString().Length == 32))
                return;

            var infohash = DecodeToHex(query["info_hash"]);
            if (infohash.Length != 40) return;

            var peerstr = query["peer_id"].ToString();
            var head = peerstr.Substring(0, 16);
            //todo: check peer-id head client 
            var peerid = DecodeToHex(peerstr.Substring(16));
            if (peerid.Length != 24) return;

            int port;
            if (!int.TryParse(query["port"], out port) || port > 0xffff) return;
            
            int rsize = 50;
            if (query.ContainsKey("numwant"))
            {
                int.TryParse(query["numwant"], out rsize);
            }

            int left=0;
            bool isSeeder;
            if (!int.TryParse(query["left"], out left)) return;
            isSeeder = left == 0;

            var user = await _userManager.FindUserByPassKey(query["passkey"]);
            if (user == null) return;

            var torrent = _dbContext.Torrents.FirstOrDefault(t => t.InfoHash == infohash);
            if (torrent == null) return;

            int peerNum;
            if (isSeeder)
            {
                peerNum = torrent.Leecher;
            }else
            {
                peerNum = torrent.Leecher + torrent.Seeder;
            }
            peerNum = peerNum > rsize ? rsize : peerNum;

            var random = new Random();
            var peerlist = _dbContext.Peers.Where(
                p => p.TorrentId == torrent.Id &&
                p.Connectable &&
                (isSeeder ? !p.IsSeeder : p.IsSeeder)).OrderBy(p => random.Next()).Take(peerNum);

            peerlist = (from p in _dbContext.Peers
                        where p.TorrentId == torrent.Id &&
                        p.Connectable &&
                        (isSeeder ? !p.IsSeeder : p.IsSeeder)
                        orderby random.Next()
                        select p).Take(peerNum);

            int interval = 60, minInterval = 30;

            var repDic = new BDictionary();

            string resstr = "d8:intervali" + interval + "e12:min intervali" + minInterval + "e8:complete" + torrent.Seeder + "e10:incomplete" + torrent.Leecher + "e5:peers";

            Peer self = null;
            string hosts = "";

            //compact
            foreach(var peer in peerlist)
            {
                if (peerid == peer.PeerId) { self = peer; continue; }
                hosts += BitConverter.ToString(IPAddress.Parse(peer.IP).GetAddressBytes()).Replace("-","") + peer.Port.ToString("x");
            }
            resstr += (6 * peerlist.Count()) + ":" + hosts + "e";

            if (self == null)
                self = _dbContext.Peers.FirstOrDefault(p => p.TorrentId == torrent.Id && p.PeerId == peerid);

            if (self != null && (DateTimeOffset.Now - self.PrevAction).Minutes < minInterval)
                return;


            //Many works to do..

        }

        public string DecodeToHex(string str)
        {
            var s = "";
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '%')
                {
                    s = s + str[++i] + str[++i];
                    continue;
                }
                if (str[i] == '\\' && str[i + 1] == 'u')
                {
                    s = s + str[i + 4] + str[i + 5];
                    i += 5;
                    continue;
                }

                s += ((int)str[i]).ToString("x");
            }
            return s.ToLower();
        }

        public string BencodeToString(BDictionary dic)
        {
            return dic.Encode();
        }


    }
}
