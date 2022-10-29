using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.IO
{
    public interface IRRBotModuleIO
    {
        T ReadJson<T>(string moduleId, string path, string fileName);
        bool SaveJson(string moduleId, string path, string fileName, object serializableObject);
    }
}
