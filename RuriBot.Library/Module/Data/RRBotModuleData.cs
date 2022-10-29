using RuriBot.Library.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleDataBase<T>
    {
        public T data;

        protected bool loaded = false;
        protected string module_id;
        protected string data_path;
        protected string data_filename;
        private IRRBotModuleIO moduleIO;
        public RRBotModuleDataBase() { loaded = false; }

        public void Init(string _moduleId, string _dataPath, string _dataFilename, IRRBotModuleIO _moduleIO)
        {
            module_id = _moduleId;
            data_path = _dataPath;
            data_filename = _dataFilename;
            moduleIO = _moduleIO;

            data = Activator.CreateInstance<T>();

            ReadData();
            SaveData();

            loaded = true;
        }

        public void ReadData()
        {
            var tempData = moduleIO.ReadJson<T>(module_id, data_path, data_filename);
            if (tempData != null) data = tempData;

            loaded = true;
        }

        public void SaveData()
        {
            moduleIO.SaveJson(module_id, data_path, data_filename, data);
        }
    }
}
