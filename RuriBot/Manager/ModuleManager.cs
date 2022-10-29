using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using RuriBot.Library.Data;
using RuriBot.Library.Module;
using CqHttpSharp.Data;
using RuriBot.Library.Event;
using CqHttpSharp.Event.Manager;
using RuriBot.Core.IO;
using RuriBot.Core.Data;
using RuriBot.Library.IO;
using CqHttpSharp.API;
using CqHttpSharp.Message;
using RuriBot.Library.Log;

namespace RuriBot.Core.Manager
{
    public class ModuleManager
    {
        private Dictionary<string, RRBotModuleEntry> modules;

        //模块操作
        protected RRBotCommandReceiver commandReceiver;
        protected CqHttpEventManager eventManager;
        protected CqHttpAPI api;

        //文件操作
        protected IRRBotModuleIO moduleIO;
        protected BotCorePermission permission;

        //模块路径
        protected string modulePath;
        protected string moduleDataPath;

        //Log
        protected IRRBotLogger coreLogger;

        public int ModuleCount
        {
            get
            {
                return modules.Count;
            }
        }

        public ModuleManager(RRBotCommandReceiver _cmdRecv, CqHttpEventManager _evtMgr, CqHttpAPI _api, IRRBotModuleIO _moduleIO, string _modulePath, string _moduleDataPath, BotCorePermission _permission, IRRBotLogger _logger = null)
        {
            commandReceiver = _cmdRecv;
            eventManager = _evtMgr;
            api = _api;
            moduleIO = _moduleIO;
            modulePath = _modulePath;
            moduleDataPath = _moduleDataPath;
            permission = _permission;

            coreLogger = _logger;

            modules = new Dictionary<string, RRBotModuleEntry>();

            commandReceiver.OnReceivePrivateBotCommand += PrivateCommandProcess;
            commandReceiver.OnReceiveGroupBotCommand += GroupCommandProcess;
        }

        ~ModuleManager()
        {
            commandReceiver.OnReceivePrivateBotCommand -= PrivateCommandProcess;
            commandReceiver.OnReceiveGroupBotCommand -= GroupCommandProcess;
        }

        public void LoadModules()
        {
            if (!Directory.Exists(modulePath)) Directory.CreateDirectory(modulePath);

            string[] module_dlls = Directory.GetFiles(modulePath);
            coreLogger?.Log("[Ruri-Bot] 开始加载模块");
            coreLogger?.Log("[Ruri-Bot] ----------");

            int loadedCount = 0;

            foreach (string dll in module_dlls)
            {
                try
                {
                    Assembly mds = Assembly.LoadFrom(dll);
                    Type[] types = mds.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsSubclassOf(typeof(RRBotModuleEntry)))
                        {
                            object o = Activator.CreateInstance(t);
                            RRBotModuleEntry module = (RRBotModuleEntry)o;
                            module.ModuleEntryInit(commandReceiver, eventManager, api, moduleIO, coreLogger, moduleDataPath);
                            if (module.ModuleFullID == "" || modules.ContainsKey(module.ModuleFullID))
                                continue; //log something when conflict
                            else
                            {
                                modules.Add(module.ModuleFullID, module);
                                coreLogger?.Log($"[Ruri-Bot] {module.ModuleName} {module.ModuleVersion} ({module.ModuleFullID}) by {module.ModuleAuthor}");
                                loadedCount++;
                            }
                            break;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            coreLogger?.Log("[Ruri-Bot] ----------");
            coreLogger?.Log($"[Ruri-Bot] 成功加载了 {loadedCount} 个模块");
        }

        protected void PrivateCommandProcess(RRBotDataCommand command, CqHttpMessagePrivate source)
        {
            /*if (!permission.IsSuperUser(source.sender.user_id)) return;

            if (command.CommandType == "rrbot" && command.CommandSubType == "modules")
            {
                if (command.CommandArgsCount == 0)
                {
                    CqMessageChain ret = new CqMessageChain();
                    ret.Builder.AddText("[RuriBot]\n");
                    ret.Builder.AddText($"已加载模块个数：{ModuleCount}\n");
                    ret.Builder.AddText("-------\n");
                    int index = 0;
                    foreach (var mod in modules)
                    {

                        if (index < ModuleCount - 1) ret.Builder.AddText("\n");
                    }
                }
            }*/
        }

        protected async void GroupCommandProcess(RRBotDataCommand command, CqHttpMessageGroup source)
        {
            if (!permission.IsSuperUser(source.sender.user_id)) return;

            if (command.CommandType == "rrbot")
            {
                if (command.CommandSubType == "modules")
                {
                    if (command.CommandArgsCount == 0)
                    {
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"已加载模块个数：{ModuleCount}\n");
                        ret.Builder.AddText("-------\n");
                        int index = 0;
                        foreach (var mod in modules)
                        {
                            ret.Builder.AddText(mod.Key);
                            if (index < ModuleCount - 1) ret.Builder.AddText("\n");
                            index++;
                        }

                        await api.SendGroupMessage(source.group_id, ret);
                    }
                }
                else if (command.CommandSubType == "set")
                {
                    if (command.CommandArgsCount > 0)
                    {
                        if (command.CommandArgs[0] == "group" && command.CommandArgsCount >= 2)
                        {
                            if (command.CommandArgs[1] == "enable" && command.CommandArgsCount == 3)
                            {
                                var moduleId = command.CommandArgs[2];
                                /*if (long.TryParse(command.CommandArgs[3]))*/
                                if (modules.ContainsKey(moduleId))
                                {
                                    if (!modules[moduleId].Permission.IsGroupPermission(source.group_id))
                                        modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.blacklist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, source.group_id);
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain($"已在本群启用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})"));
                                }
                                else
                                {
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain("模块 ID 无效，可能是未加载或输入错误"));
                                }
                            }
                            else if (command.CommandArgs[1] == "enable" && command.CommandArgsCount == 4)
                            {
                                var moduleId = command.CommandArgs[2];
                                if (long.TryParse(command.CommandArgs[3], out var targetGroupId))
                                {
                                    if (modules.ContainsKey(moduleId))
                                    {
                                        if (!modules[moduleId].Permission.IsGroupPermission(targetGroupId))
                                            modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.blacklist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, targetGroupId);
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain($"已在群 {targetGroupId} 启用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})"));
                                    }
                                    else
                                    {
                                        await api.SendGroupMessage(source.group_id, new CqMessageChain("模块 ID 无效，可能是未加载或输入错误"));
                                    }
                                }
                                else
                                {
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain("群号无效"));
                                }
                            }
                            else if (command.CommandArgs[1] == "disable" && command.CommandArgsCount == 3)
                            {
                                var moduleId = command.CommandArgs[2];
                                /*if (long.TryParse(command.CommandArgs[3]))*/
                                if (modules.ContainsKey(moduleId))
                                {
                                    if (modules[moduleId].Permission.IsGroupPermission(source.group_id))
                                        modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.whitelist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, source.group_id);
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain($"已在本群禁用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})"));
                                }
                                else
                                {
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain("模块 ID 无效，可能是未加载或输入错误"));
                                }
                            }
                            else if (command.CommandArgs[1] == "disable" && command.CommandArgsCount == 4)
                            {
                                var moduleId = command.CommandArgs[2];
                                if (long.TryParse(command.CommandArgs[3], out var targetGroupId))
                                {
                                    if (modules.ContainsKey(moduleId))
                                    {
                                        if (modules[moduleId].Permission.IsGroupPermission(targetGroupId))
                                            modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.whitelist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, targetGroupId);
                                        await api.SendGroupMessage(source.group_id, new CqMessageChain($"已在群 {targetGroupId} 禁用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})"));
                                    }
                                    else
                                    {
                                        await api.SendGroupMessage(source.group_id, new CqMessageChain("模块 ID 无效，可能是未加载或输入错误"));
                                    }
                                }
                                else
                                {
                                    await api.SendGroupMessage(source.group_id, new CqMessageChain("群号无效"));
                                }
                            }
                        }
                        else
                        {

                        }
                        
                    }
                }
            }
        }
    }
}
