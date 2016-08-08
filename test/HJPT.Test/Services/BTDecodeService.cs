using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HJPT.Test.Services
{
    public class BTDecodeServiceTest : TestBase
    {

        private readonly ITestOutputHelper output;
        public BTDecodeServiceTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("3:abci123ee")]
        public void DecodeList(string str)
        {
            var ds = new BTDecodeService(str);
            
            Assert.Equal(new List<object>() { "abc",123}, ds.DecodeList());
        }

        [Theory]
        [InlineData("4:name11:create chen3:agei23ee")]
        public void DecodeDic(string str)
        {
            var ds = new BTDecodeService(str);

            var result = ds.DecodeDic();
            Assert.NotNull(result);
            output.WriteLine("!!!!");
        }

        [Fact]
        public void DecodeTorrent()
        {
            var ds = new BTDecodeService(new FileStream("Data/Torrents/111.torrent", FileMode.Open));

            var result = ds.DecodeTorrent();

            //Debug.Write(ToJson(result));
            Write((result["info"] as Dictionary<string, object>)["source"] as string);
            Assert.NotNull(result);
        }


        public class BTDecodeService : IDisposable
        {
            private Stream stream;

            public BTDecodeService(FileStream stream)
            {
                this.stream = stream;
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
                int num = 0,x;
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

            public Dictionary<string, object> DecodeTorrent()
            {
                if (stream.ReadByte() != 'd') return null;
                return DecodeDic();
            }
        }

    }
}
