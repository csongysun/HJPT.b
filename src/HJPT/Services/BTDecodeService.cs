using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJPT.Services
{
    public interface IBTDecodeService
    {
        Dictionary<string, object> DecodeTorrent(Stream stream);
    }

    public class BTDecodeService : IBTDecodeService
    {
        private Stream stream;

        public string DecodeString(int len)
        {
            //StringBuilder sb = new StringBuilder();
            var bl = new List<byte>();
            var bs = new byte[len];
            stream.ReadByte();
            var tot = stream.Read(bs, 0, len);
            //while (len-- > 0)
            //{
            //}
            return Encoding.UTF8.GetString(bs);
        }

        public int DecodeStringLen(int first)
        {
            int len = first, x;
            while (true)
            {
                x = stream.ReadByte();
                if (x == ':') break;
                len *= 10;
                len += x - 48;
            }
            --stream.Position;
            return len;
        }

        public int DecodeNumber()
        {
            int num = 0, x;
            while (true)
            {
                x = stream.ReadByte();
                if (x == 'e') break;
                num *= 10;
                num += x - 48;
            }

            return num;
        }

        public List<object> DecodeList()
        {
            var list = new List<object>();
            while (true)
            {
                if (stream.ReadByte() == 'e') break;
                --stream.Position;
                list.Add(Decode());
            }
            return list;
        }

        public object Decode()
        {
            var p = stream.ReadByte();
            if (p == 'l')
            {
                return DecodeList();
            }
            if (p == 'i')
            {
                return DecodeNumber();
            }
            if (p >= '0' && p <= '9')
            {
                return DecodeString(DecodeStringLen(p - 48));
            }
            if (p == 'd')
            {
                return DecodeDic();
            }
            return null;
        }

        public Dictionary<string, object> DecodeDic()
        {
            var dic = new Dictionary<string, object>();
            while (true)
            {
                var first = stream.ReadByte();
                if (first == 'e') break;
                var key = DecodeString(DecodeStringLen(first - 48));
                var value = Decode();
                dic.Add(key, value);
            }
            return dic;
        }

        public Dictionary<string, object> DecodeTorrent(Stream stream)
        {
            this.stream = stream;
            if (stream.ReadByte() != 'd') return null;
            return DecodeDic();
        }
    }

}
