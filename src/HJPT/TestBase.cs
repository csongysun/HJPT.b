using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HJPT
{
    public class TestBase
    {
        

        public void Write(object obj)
        {
            var jo = (JObject)JToken.FromObject(obj);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings());

            using (StreamWriter sw = File.CreateText("obj.json"))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jo.WriteTo(jw);
            }
        }

        public void Write(string str)
        {
            using (StreamWriter sw = File.CreateText("obj.json"))
            {
                sw.WriteLine(str);
                sw.Flush();
            }
        }

        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }
}
