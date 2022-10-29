using System;
using System.IO;
using System.Reflection;
using CqHttpSharp.WebSocket;
using CqHttpSharp.Data;
using CqHttpSharp.API;
using RuriBot.Library.Event;
using RuriBot.Core.Manager;
using RuriBot.Core.Data;
using RuriBot.Core.Lexer;
using RuriBot.Core.IO;
using RuriBot.Library.Log;
using RuriBot.Core.Const;
using CqHttpSharp.Log;

namespace RuriBot.Core
{
    public class RRBotCore
    {
        //WS通信核心
        CqHttpWebSocket webSocket;

        //输出
        IRRBotLogger coreLogger;

        //API交互相关
        CqHttpAPI api;
        CqHttpAPIAdvanced apiAdvanced;

        //IO相关
        string assemblyExecutePath;
        CoreIO coreIO;
        ModuleIO moduleIO;

        //保存文件
        BotCorePermission permission;

        //模块相关
        string modulePath;
        string moduleDataPath;
        ModuleManager moduleManager;

        //词法分析
        RRBotCommandReceiver commandReceiver;
        CommandLexer commandLexer;

        public RRBotCore(string _ip, int _port, IRRBotLogger _logger = null)
        {
            webSocket = new CqHttpWebSocket(_ip, _port);
            api = new CqHttpAPI(webSocket);
            apiAdvanced = new CqHttpAPIAdvanced(webSocket);

            coreLogger = _logger;

            commandReceiver = new RRBotCommandReceiver();
            commandLexer = new CommandLexer();

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
            permission = new BotCorePermission("permission", "permission", coreIO);

            OutputBotLog("Ruri-Bot", "数据初始化完成");
        }

        private void InitModule()
        {
            modulePath = Path.Combine(assemblyExecutePath, BotCoreConst.ModulePath);
            moduleDataPath = Path.Combine(assemblyExecutePath, BotCoreConst.ModuleDataPath);

            moduleManager = new ModuleManager(commandReceiver, webSocket.EventManager, api, moduleIO, modulePath, moduleDataPath, permission, coreLogger);
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
            webSocket.EventManager.OnEventMessagePrivate += ProcessPrivateMessage;
            webSocket.EventManager.OnEventMessageGroup += ProcessGroupMessage;
            api.APILogger += OutputBotLog;
            webSocket.EventManager.EventLogger += OutputBotLog;
        }

        private void EventUnregistry()
        {
            webSocket.EventManager.OnEventMessagePrivate -= ProcessPrivateMessage;
            webSocket.EventManager.OnEventMessageGroup -= ProcessGroupMessage;
            api.APILogger -= OutputBotLog;
            webSocket.EventManager.EventLogger -= OutputBotLog;
        }

        private void ProcessPrivateMessage(CqHttpMessagePrivate msg)
        {
            var cmd = commandLexer.MessageLexer(msg.message);
            if (cmd != null)
                commandReceiver.OnReceivePrivateBotCommand?.Invoke(cmd, msg);
        }

        private void ProcessGroupMessage(CqHttpMessageGroup msg)
        {
            var cmd = commandLexer.MessageLexer(msg.message);
            if (cmd != null)
                commandReceiver.OnReceiveGroupBotCommand?.Invoke(cmd, msg);
        }

        private void OutputBotLog(string prefix, string content)
        {
            coreLogger?.Log($"[{prefix}] {content}");
        }

        private void OutputBotLog(string content, CqHttpLogType type)
        {
            coreLogger?.Log("[Ruri-Bot] " + content);
        }
    }
}
