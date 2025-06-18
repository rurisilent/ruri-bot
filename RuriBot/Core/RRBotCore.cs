using System;
using System.IO;
using System.Reflection;
using RuriBot.Library.Event;
using RuriBot.Core.Manager;
using RuriBot.Core.Data;
using RuriBot.Core.Lexer;
using RuriBot.Core.IO;
using RuriBot.Library.Log;
using RuriBot.Core.Const;
using NapCatSharpLib.WebSocket;
using NapCatSharpLib.API;
using RuriBot.Core.Permission;
using NapCatSharpLib.Log;
using NapCatSharpLib.Data;
using RuriBot.Library.Module;
using System.Collections.Generic;

namespace RuriBot.Core
{
    public class RRBotCore
    {
        class DebugWSOutput : INapCatWSDebugOutput
        {
            IRRBotLogger logger;

            public DebugWSOutput(IRRBotLogger _logger)
            {
                logger = _logger;
            }

            public void Log(string message)
            {
                logger.Log(message);
            }
        }

        class ModulePermissionDummy : IRRBotModulePermissionOperation
        {
            public void ClearGroupPermission()
            {
                throw new NotImplementedException();
            }

            public void ClearPrivatePermission()
            {
                throw new NotImplementedException();
            }

            public bool IsAdmin(long id)
            {
                throw new NotImplementedException();
            }

            public bool IsGroupPermission(long id)
            {
                return true;
            }

            public bool IsPrivatePermission(long id)
            {
                return true;
            }

            public void SetAdmin(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetAdmin(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermission(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermission(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermissionType(RRBotModulePermissionType type, bool clearPermission = true)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermission(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermission(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermissionType(RRBotModulePermissionType type, bool clearPermission = true)
            {
                throw new NotImplementedException();
            }
        }

        //WS通信核心
        NapCatWebSocket webSocket;

        //输出
        IRRBotLogger coreLogger;

        //API交互相关
        NapCatAPI api;
        NapCatAPIAdvanced apiAdvanced;

        //IO相关
        string assemblyExecutePath;
        CoreIO coreIO;
        ModuleIO moduleIO;

        //模块与权限相关
        string modulePath;
        string moduleDataPath;
        ModuleManager moduleManager;
        PermissionManager permissionManager;
        ModulePermissionDummy permissionDummy;

        //事件广播
        CommandManager commandManager;
        EventManager eventManager;
        CommandLexer commandLexer;

        public RRBotCore(string _ip, int _port, IRRBotLogger _logger = null)
        {
            webSocket = new NapCatWebSocket(_ip, _port);
            api = new NapCatAPI(webSocket);
            apiAdvanced = new NapCatAPIAdvanced(webSocket);

            coreLogger = _logger;

            commandManager = new CommandManager();
            eventManager = new EventManager(webSocket.EventManager);
            commandLexer = new CommandLexer();

            permissionDummy = new ModulePermissionDummy();

            Init();
            InitData();
            InitModule();

            EventRegistry();
        }

        ~RRBotCore()
        {
            EventUnregistry();
        }

        private void Init()
        {
            //Init IO
            assemblyExecutePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            coreIO = new CoreIO(Path.Combine(assemblyExecutePath, BotCoreConst.CoreDataPath));
            moduleIO = new ModuleIO(Path.Combine(assemblyExecutePath, BotCoreConst.ModuleDataPath));

            OutputBotLog("Ruri-Bot", "功能初始化完成");
        }

        private void InitData()
        {
            permissionManager = new PermissionManager(coreIO, commandManager);

            OutputBotLog("Ruri-Bot", "数据初始化完成");
        }

        private void InitModule()
        {
            modulePath = Path.Combine(assemblyExecutePath, BotCoreConst.ModulePath);
            moduleDataPath = Path.Combine(assemblyExecutePath, BotCoreConst.ModuleDataPath);

            moduleManager = new ModuleManager(commandManager, eventManager, api, moduleIO, modulePath, moduleDataPath, permissionManager, coreLogger);
            moduleManager.LoadModules();

            OutputBotLog("Ruri-Bot", "模块初始化完成");
        }

        public void Start()
        {
            webSocket.Start();
            OutputBotLog("Ruri-Bot", "核心启动");
        }

        private void EventRegistry()
        {
            api.LogAPI += OutputBotLog;
            webSocket.EventManager.EventLogger += OutputBotLog;

            eventManager.Register<NapCatMessagePrivate>(permissionDummy, ProcessPrivateMessage);
            eventManager.Register<NapCatMessageGroup>(permissionDummy, ProcessGroupMessage);
        }

        private void EventUnregistry()
        {
            api.LogAPI -= OutputBotLog;
            webSocket.EventManager.EventLogger -= OutputBotLog;

            eventManager.Unregister<NapCatMessagePrivate>(permissionDummy, ProcessPrivateMessage);
            eventManager.Unregister<NapCatMessageGroup>(permissionDummy, ProcessGroupMessage);
        }

        private void ProcessPrivateMessage(NapCatMessagePrivate msg)
        {
            var cmd = commandLexer.MessageLexer(msg.message);
            if (cmd != null)
                commandManager.ReactPrivate(cmd, msg, out string ret);
        }

        private void ProcessGroupMessage(NapCatMessageGroup msg)
        {
            var cmd = commandLexer.MessageLexer(msg.message);
            if (cmd != null)
                commandManager.ReactGroup(cmd, msg, out string ret);
        }

        private void OutputBotLog(string prefix, string content)
        {
            coreLogger?.Log($"[{prefix}] {content}");
        }

        private void OutputBotLog(string content, NapCatLogType type)
        {
            coreLogger?.Log("[Ruri-Bot] " + content);
        }
    }
}
