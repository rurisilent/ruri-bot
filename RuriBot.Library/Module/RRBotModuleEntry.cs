using CqHttpSharp.API;
using CqHttpSharp.Data;
using CqHttpSharp.Event.Manager;
using RuriBot.Library.Data;
using RuriBot.Library.Event;
using RuriBot.Library.IO;
using RuriBot.Library.Log;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleEntry
    {
        //模块操作
        protected RRBotCommandReceiver commandReceiver;
        protected CqHttpEventManager eventManager;
        protected CqHttpAPI api;

        //文件操作
        protected IRRBotModuleIO fileIO;
        protected string moduleDataPath;

        //Log
        protected IRRBotLogger coreLogger;

        //数据
        public RRBotModulePermission Permission { get; protected set; }

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
            if (commandReceiver != null)
            {
                commandReceiver.OnReceivePrivateBotCommand -= PrivateCommandProcess;
                commandReceiver.OnReceiveGroupBotCommand -= GroupCommandProcess;
            }
        }

        public void ModuleEntryInit(RRBotCommandReceiver _cmdRecv, CqHttpEventManager _evtMgr, CqHttpAPI _api, IRRBotModuleIO _io, IRRBotLogger _logger, string _moduleDataPath)
        {
            commandReceiver = _cmdRecv;
            eventManager = _evtMgr;
            api = _api;
            fileIO = _io;

            coreLogger = _logger;

            SetModuleInfo();
            ConfigurationInit();

            moduleDataPath = _moduleDataPath;
            moduleDataPath = Path.Combine(moduleDataPath, ModuleFullID);

            ExtendedInit();

            commandReceiver.OnReceivePrivateBotCommand += PrivateCommandProcess;
            commandReceiver.OnReceiveGroupBotCommand += GroupCommandProcess;
        }

        protected void Log(string content)
        {
            coreLogger?.Log($"[{module_id}] {content}");
        }

        protected abstract void SetModuleInfo();
        protected abstract void ConfigurationInit();
        protected virtual void ExtendedInit() { }
        protected abstract void PrivateCommandProcess(RRBotDataCommand command, CqHttpMessagePrivate source);
        protected abstract void GroupCommandProcess(RRBotDataCommand command, CqHttpMessageGroup source);
    }
}
