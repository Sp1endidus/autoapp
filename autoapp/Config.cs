using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoApp
{
    class Config
    {
        private string path;
        public Dictionary<string, string> Values { private set; get; }
        public bool ConfigLoadedSuccessful { private set; get; }

        public Config(string path, bool loadNow)
        {
            this.path = path;

            if (loadNow)
                ConfigLoadedSuccessful = LoadConfig();
        }

        public Config(string path) : this(path, false) { }

        public bool LoadConfig()
        {
            Values = new Dictionary<string, string>();
            try
            {
                string[] rawData = File.ReadAllLines(path);
                for (int i = 0; i < rawData.Length; i++)
                {
                    if (rawData[i].Contains('='))
                    {
                        string key = rawData[i].Substring(0, rawData[i].IndexOf('='));
                        if (!Values.ContainsKey(key))
                        {
                            string value = rawData[i].Substring(rawData[i].IndexOf('=') + 1);
                            Values.Add(key, value);
                        }
                    }
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                Log.L("Config " + path + " not found!");
                return false;
            }
            catch (Exception e)
            {
                Log.L("Load config error!\r\n" + e.ToString());
                return false;
            }
        }

        public T GetValue<T>(string key)
        {
            string rawResult;
            if (Values.TryGetValue(key, out rawResult))
            {
                try
                {
                    T result = (T)Convert.ChangeType(rawResult, typeof(T));
                    return result;
                }
                catch
                {
                    Log.L("Type cast error!");
                }
            }
            else
            {
                Log.L("Key " + key + " doesn't exist!");
            }
            return (T)Convert.ChangeType(new byte(), typeof(T));
        }
    }
}
