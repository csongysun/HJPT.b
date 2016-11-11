using BencodeNET.Objects;
using HJPT.Data;
using HJPT.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Csys.Identity;

namespace HJPT.Services
{
    public interface ITrackService
    {
        Task<string> Accept(HttpRequest req);
    }

    public class TrackService : ITrackService
    {
        private readonly UserManager _user;
        private readonly ApplicationDbContext _dbContext;
        public TrackService(UserManager userManager, ApplicationDbContext dbContext)
        {
            _user = userManager;
            _dbContext = dbContext;
        }

        //Todo: tracker accept
        public async Task<string> Accept(HttpRequest req)
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
                return null;

            var infohash = DecodeToHex(query["info_hash"]);
            if (infohash.Length != 40) return null;
            
            var peerstr = query["peer_id"].ToString();
            var head = peerstr.Substring(0, 16);
            //todo: check peer-id head client 
            var peerid = DecodeToHex(peerstr.Substring(16));
            if (peerid.Length != 24) return null;

            int port;
            if (!int.TryParse(query["port"], out port) || port > 0xffff) return null;
            
            int rsize = 50;
            if (query.ContainsKey("numwant"))
            {
                int.TryParse(query["numwant"], out rsize);
            }

            int left=0;
            bool isSeeder;
            if (!int.TryParse(query["left"], out left)) return null;
            isSeeder = left == 0;

            var user = await _user.FindUserByPassKey(query["passkey"]);
            if (user == null) return null;

            var torrent = _dbContext.Torrents.FirstOrDefault(t => t.InfoHash == infohash);
            if (torrent == null) return null;

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

            //var repDic = new BDictionary();

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
                return null;


            //todo: valid check

            var evt = query["event"];

            //started
            if (self == null && evt == "start")
            {
                var peer = new Peer() {
                    TorrentId = torrent.Id,
                    //...
                };
                if (isSeeder) ++torrent.Seeder;
                else ++torrent.Leecher;
                await _dbContext.SaveChangesAsync();
            }

            //Many works to do..


            return resstr;

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
