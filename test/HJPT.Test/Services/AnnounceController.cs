using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HJPT.Test.Services
{

    public class AnnounceControllerTest : HJPT.Test.TestBase
    {

        [Fact]
        public void BitConvertFromString()
        {
            var str = "%99%D7%E9%A0v%E2%C8-|%E2%F7vZ%BD\\u000f%EF\\u001bl%EF%B7";
            string str2 = "";
            for(int i = 0; i < str.Length; ++i)
            {
                if(str[i] == '%')
                {
                    str2 = str2 + str[++i] + str[++i];
                    continue;
                }
                if(str[i] == '\\' && str[i+1] == 'u')
                {
                    str2 = str2 + str[i + 4] + str[i + 5];
                    i += 5;
                    continue;
                }

                str2 += ((int) str[i]).ToString("x");
            }

            var hash = "99D7E9A076E2C82D7CE2F7765ABD0FEF1B6CEFB7";
            //Write(str2);

            Assert.Equal(hash.ToLower(), str2.ToLower());
           
        }

        [Fact]
        public void HostEncode()
        {
            var sip = new IPAddress(new byte[4] { 0x3a, 0x75, 0xb5, 0x96});
            var ip = IPAddress.Parse("192.168.1.105");
            var bs = ip.GetAddressBytes();

            var str = BitConverter.ToString(sip.GetAddressBytes()) + 10756.ToString("x");
            ;
        }

    }
}
