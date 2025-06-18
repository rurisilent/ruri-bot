using RuriBot.Library.Data;
using RuriBot.Library.Event;
using RuriBot.Library.IO;
using RuriBot.Library.Log;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NapCatSharpLib.API;
using NapCatSharpLib.Event.Manager;
using RuriBot.Library.Permission;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleEntry
    {
        //模块操作
        protected IRRBotCommandRegistry cmdRegister;
        protected IRRBotEventRegistry evtRegister;
        protected NapCatAPI api;

        //文件操作
        protected IRRBotModuleIO fileIO;
        protected string moduleDataPath;

        //Log
        protected IRRBotLogger coreLogger;

        //数据
        public RRBotModulePermission Permission { get; protected set; }
        public IRRBotPermission BotPermission { get; protected set; }

        //只读
        protected string module_id = "";
        protected string module_subid = "";
        protected string module_name = "";
        protected string module_version = "1.0.0.0";
        protected string module_author = "";

        //属性
        public string ModuleID { get { return module_id; } }
        public string ModuleSubID { get { return module_subid; } }
        public string ModuleFullID
        {
            get
            {
                if (module_subid != "") return $"{module_id}_{module_subid}";
                else return module_id;
            }
        }
        public string ModuleName { get { return module_name; } }
        public string ModuleVersion { get { return module_version; } }
        public string ModuleAuthor { get { return module_author; } }
        public IRRBotModulePermissionOperation ModulePermissionInterface
        {
            get
            {
                return Permission;
            }
        }

        ~RRBotModuleEntry()
        {
            UnregisterPrivateCommands();
            UnregisterGroupCommands();
            UnregisterNapCatEvents();
        }

        public void ModuleEntryInit(IRRBotCommandRegistry _cmdReg, IRRBotEventRegistry _evtReg, NapCatAPI _api, IRRBotModuleIO _io, IRRBotPermission _botPerm, IRRBotLogger _logger, string _moduleDataPath)
        {
            cmdRegister = _cmdReg;
            evtRegister = _evtReg;
            api = _api;
            fileIO = _io;
            BotPermission = _botPerm;

            coreLogger = _logger;

            SetModuleInfo();
            ConfigurationInit();

            moduleDataPath = _moduleDataPath;
            moduleDataPath = Path.Combine(moduleDataPath, ModuleFullID);

            ExtendedInit();
            RegisterPrivateCommands();
            RegisterGroupCommands();
            RegisterNapCatEvents();
        }

        protected void Log(string content)
        {
            coreLogger?.Log($"[{module_id}] {content}");
        }

        protected abstract void SetModuleInfo();
        protected abstract void ConfigurationInit();
        protected virtual void ExtendedInit() { }
        protected abstract void RegisterPrivateCommands();
        protected abstract void UnregisterPrivateCommands();
        protected abstract void RegisterGroupCommands();
        protected abstract void UnregisterGroupCommands();
        protected abstract void RegisterNapCatEvents();
        protected abstract void UnregisterNapCatEvents();
    }
}
