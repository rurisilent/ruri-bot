using RuriBot.Library.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleConfigBase<T>
    {
        public T data;

        protected bool loaded = false;
        protected string module_id;
        private IRRBotModuleIO moduleIO;

        public RRBotModuleConfigBase() { loaded = false; }

        public void Init(string _moduleId, IRRBotModuleIO _moduleIO)
        {
            module_id = _moduleId;
            moduleIO = _moduleIO;

            data = Activator.CreateInstance<T>();

            ReadData();
            SaveData();

            loaded = true;
        }

        public void ReadData()
        {
            var tempData = moduleIO.ReadJson<T>(module_id, "", "config");
            if (tempData != null) data = tempData;

            loaded = true;
        }

        public void SaveData()
        {
            moduleIO.SaveJson(module_id, "", "config", data);
        }
    }
}
