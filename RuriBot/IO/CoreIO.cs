using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RuriBot.Library.IO;

namespace RuriBot.Core.IO
{
    public class CoreIO : IRRBotCoreIO
    {
        private string basePath;

        public CoreIO(string _basePath)
        {
            basePath = _basePath;
        }

        public T ReadJson<T>(string path, string fileName)
        {
            string finalPath;
            if (path != "") finalPath = Path.Combine(basePath, path, $"{fileName}.json");
            else finalPath = Path.Combine(basePath, $"{fileName}.json");

            if (File.Exists(finalPath))
            {
                string data = File.ReadAllText(finalPath);
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                return default(T);
            }
        }

        public bool SaveJson(string path, string fileName, object serializableObject)
        {
            //check directory existence
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            if (path != "" && !Directory.Exists(Path.Combine(basePath, path))) Directory.CreateDirectory(Path.Combine(basePath, path));

            string finalPath;
            if (path != "") finalPath = Path.Combine(basePath, path, $"{fileName}.json");
            else finalPath = Path.Combine(basePath, $"{fileName}.json");

            try
            {
                File.WriteAllText(finalPath, JsonConvert.SerializeObject(serializableObject, Formatting.Indented));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"RuriBot I/O Error: {e.Message} \n {e.StackTrace}");
                return false;
            }
        }
    }
}
