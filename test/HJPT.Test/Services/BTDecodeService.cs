using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HJPT.Test.Services
{
    public class BTDecodeServiceTest : TestBase
    {

        [Theory]
        [InlineData("Data/Torrents/111.torrent")]
        public void TorrentInfoHash(string path)
        {
            var bs = new BTDecodeService(new FileStream(path, FileMode.Open));
            var torrent = bs.DecodeTorrent();

            Write(torrent);
        }

        public class BTDecodeService : IDisposable
        {
            private Stream stream;
            private int currentByte;

            public BTDecodeService(FileStream stream)
            {
                this.stream = stream;
                stream.Position = 0;
            }
            public BTDecodeService(byte[] bs)
            {
                stream = new MemoryStream();
                stream.Write(bs, 0, bs.Length);
                stream.Position = 0;
            }

            public void Dispose()
            {
                stream.Dispose();
            }

            public BTDecodeService(string str)
            {
                var b = Encoding.ASCII.GetBytes(str);
                stream = new MemoryStream(b);
            }

            public string DecodeString(int len)
            {
                var bs = new byte[len];
                var tot = stream.Read(bs, 0, len);
                var str = Encoding.ASCII.GetString(bs);
                return Encoding.ASCII.GetString(bs);
            }

            public int DecodeStringLen()
            {
                int len = currentByte - 48;
                while (true)
                {
                    currentByte = stream.ReadByte();
                    if (currentByte == ':') break;
                    if (currentByte > '9' || currentByte < '0')
                        throw new InvalidDataException();
                    len *= 10;
                    len += currentByte - 48;
                }
                return len;
            }

            public long DecodeNumber()
            {
                long num = 0; int flag = 1;
                while (true)
                {
                    currentByte = stream.ReadByte();
                    if (currentByte == 'e') break;
                    if (currentByte > '9' || currentByte < '0')
                    {
                        if (currentByte == '-')
                        {
                            flag = -1;
                            continue;
                        }
                        throw new InvalidDataException();
                    }
                    num *= 10;
                    num += currentByte - 48;
                }

                return flag * num;
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
                currentByte = stream.ReadByte();
                if (currentByte == 'l')
                {
                    return DecodeList();
                }
                if (currentByte == 'i')
                {
                    return DecodeNumber();
                }
                if (currentByte >= '0' && currentByte <= '9')
                {
                    return DecodeString(DecodeStringLen());
                }
                if (currentByte == 'd')
                {
                    return DecodeDic();
                }
                return new InvalidDataException();
            }

            public Dictionary<string, object> DecodeDic()
            {
                var dic = new Dictionary<string, object>();
                while (true)
                {
                    currentByte = stream.ReadByte();
                    if (currentByte == 'e') break;
                    var key = DecodeString(DecodeStringLen());
                    if (key == "pieces")
                        ;
                    var value = Decode();
                    dic.Add(key, value);
                }
                return dic;
            }

            public Dictionary<string, object> DecodeTorrent()
            {
                currentByte = stream.ReadByte();
                if (currentByte != 'd')
                    throw new InvalidDataException();
                return DecodeDic();
            }

            public byte[] Encode2Byte(object obj)
            {
                string str = Encode2String(obj);
                return Encoding.ASCII.GetBytes(str);
            }

            public string Encode2String(object obj)
            {
                if (obj is Dictionary<string, object>)
                {
                    return EncodeDic(obj as Dictionary<string, object>);
                }
                if (obj is string)
                {
                    return EncodeString(obj as string);
                }
                if (obj is long)
                {
                    return EncodeNumber((long)obj);
                }
                if (obj is List<object>)
                {
                    return EncodeList(obj as List<object>);
                }
                else throw new InvalidDataException();
            }

            public string EncodeDic(Dictionary<string, object> dic)
            {
                string str = "d";
                foreach (var item in dic)
                {
                    str += EncodeString(item.Key);
                    str += Encode2String(item.Value);
                }
                return str + "e";
            }
            public string EncodeString(string s)
            {
                //var bs = Encoding.UTF8.GetBytes(s);
                return s.Length + ":" + s;
            }
            public string EncodeNumber(long x)
            {
                return "i" + x.ToString() + 'e';
            }
            public string EncodeList(List<object> list)
            {
                string str = "l";
                foreach (var item in list)
                {
                    str += Encode2String(item);
                }
                return str + "e";
            }

        }

    }
}
