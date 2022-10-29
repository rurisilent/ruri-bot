using System;
using System.Collections.Generic;
using System.Text;
using RuriBot.Core.IO;

namespace RuriBot.Core.Data
{
    public abstract class BotCoreDataBase<T>
    {
        protected T data;

        protected bool loaded = false;
        protected string data_path;
        protected string data_filename;
        private IRRBotCoreIO coreIO;

        public T Data
        {
            get { return data; }
            set
            {
                data = value;
                SaveData();
            }
        }

        public BotCoreDataBase(string _dataPath, string _dataFilename, IRRBotCoreIO _coreIO)
        {
            data_path = _dataPath;
            data_filename = _dataFilename;
            coreIO = _coreIO;

            ReadData();

            loaded = true;
        }

        protected void ReadData()
        {
            var tempData = coreIO.ReadJson<T>(data_path, data_filename);
            if (tempData != null) data = tempData;
            else
            {
                SetDefaultValue();
                SaveData();
            }

            loaded = true;
        }

        protected void SaveData()
        {
            coreIO.SaveJson(data_path, data_filename, data);
        }

        protected abstract void SetDefaultValue();
    }
}
