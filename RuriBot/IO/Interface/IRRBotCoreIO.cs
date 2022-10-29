using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.IO
{
    public interface IRRBotCoreIO
    {
        T ReadJson<T>(string path, string fileName);
        bool SaveJson(string path, string fileName, object serializableObject);
    }
}
