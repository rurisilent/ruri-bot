using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RuriBot.Library.IO;

namespace RuriBot.Core.IO
{
    public class ModuleIO : IRRBotModuleIO
    {
        private string basePath;

        public ModuleIO(string _basePath)
        {
            basePath = _basePath;
        }

        public T ReadJson<T>(string moduleId, string path, string fileName)
        {
            string finalPath;
            if (path != "") finalPath = Path.Combine(basePath, moduleId, path, $"{fileName}.json");
            else finalPath = Path.Combine(basePath, moduleId, $"{fileName}.json");

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

        public bool SaveJson(string moduleId, string path, string fileName, object serializableObject)
        {
            //check directory existence
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            if (!Directory.Exists(Path.Combine(basePath, moduleId))) Directory.CreateDirectory(Path.Combine(basePath, moduleId));
            if (path != "" && !Directory.Exists(Path.Combine(basePath, moduleId, path))) Directory.CreateDirectory(Path.Combine(basePath, moduleId, path));

            string finalPath;
            if (path != "") finalPath = Path.Combine(basePath, moduleId, path, $"{fileName}.json");
            else finalPath = Path.Combine(basePath, moduleId, $"{fileName}.json");

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
