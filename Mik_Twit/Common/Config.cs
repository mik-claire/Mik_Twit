using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mik_Twit.Common
{
    public static class ConfigManager
    {
        public static Conf Config { get; set; }

        public static void LoadConf()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var confPath = Path.Combine(baseDir, "Conf", "config.json");

            using (StreamReader sr = new StreamReader(confPath, Encoding.Default))
            {
                string rte = sr.ReadToEnd().Replace("\\", "\\\\");
                Config = JsonConvert.DeserializeObject<Conf>(rte);
            }
        }

        public static void SaveConfig()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var confPath = Path.Combine(baseDir, "Conf", "config.json");

            string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(confPath, false, Encoding.Default))
            {
                sw.WriteLine(json);
            }
        }

        public class Conf
        {
            public bool Notification = false;
        }
    }
}
